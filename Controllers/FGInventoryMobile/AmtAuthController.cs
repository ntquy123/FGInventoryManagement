using System.Collections.Generic;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using entities.Common;
using entities.Setting;
using erpsolution.dal.DTO;
using erpsolution.dal.EF;
using erpsolution.entities;
using erpsolution.entities.Common;
using erpsolution.service.Common.Cache;
using erpsolution.service.Interface;
using FGInventoryManagement.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Oracle.ManagedDataAccess.Client;
using service.Common.Base;
using service.Common.Base.Interface;
using service.Interface;
using erpsolution.dal.Context;

namespace FGInventoryManagement.Controllers.FGInventoryMobile
{
    public class AmtAuthController : ControllerBaseEx<IAmtAuthService, OspAppusrTbl, decimal>
    {
        private AmtContext _context;
        private IMapper _mapper;
        public IServiceProvider _serviceProvider;
        private readonly ICacheService _memoryCache;
        private readonly IConfiguration _config;
        public AmtAuthController(IAmtAuthService service,
       IConfiguration config,
       IServiceProvider serviceProvider,
       AmtContext context,
       ICacheService memoryCache,
       ICurrentUser currentUser) : base(service, currentUser)
        {
            _config = config;
            _memoryCache = memoryCache;
            _serviceProvider = serviceProvider;
            _context = context;
        }
        [ApiExplorerSettings(GroupName = "fg_inventory_mobile")]
        [HttpGet(nameof(GetRole))]
        [AllowAnonymous]
        public async Task<HandleState> GetRole(string UserId, string menuNm)
        {
            try
            {
                var data = await _service.GetRole(UserId, menuNm);
                return new HandleState(true, data);
            }
            catch (Exception ex)
            {
                return new HandleState(false, ex.Message);
            }
        }

        [ApiExplorerSettings(GroupName = "fg_inventory_mobile")]
        [HttpPost(nameof(LoginERP))]
        [ProducesResponseType(typeof(HandleResponse<object>), 400)]
        [AllowAnonymous]

        public IActionResult LoginERP([FromBody] LoginModel login)
        {
            var user = _service.CheckLoginERP(login);
            if (user != null)
            {
                return CreateTokenERP(user);
            }
            return Json(new HandleResponse<LoginModel>(false, "Username or password is incorrect", null));
        }
        private IActionResult CreateTokenERP(TCMUSMT user)
        {

            if (user != null)
            {
                var tokenString = BuildTokenERP(user);
                //string key = "E821752166E916AEEF940855";
                var userInfo = new
                {
                    user_id = user.UserId,
                    full_name = user.Name,
                };
                var encrypedBytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(userInfo));
                var encrypted = Convert.ToBase64String(encrypedBytes, 0, encrypedBytes.Length);

                _memoryCache.Add<TokenLifeTime>(new TokenLifeTime
                {
                    userName = user.UserId,
                    LoginTime = DateTime.Now,
                    RemoteIpAddress = Request.HttpContext.Connection.RemoteIpAddress.ToString()
                }, "TOKEN_LIFETIME_" + user.UserId, 15);
                //lib.WindowsClipboard.SetText($"Bearer {tokenString}");
                return Json(new
                {
                    success = true,
                    data = new
                    {
                        token = tokenString,
                        user_info = encrypted //EncryptUtil.TripleDesEncrypt(JsonConvert.SerializeObject(userInfo),key)
                    }
                });
            }
            //Write token into cache
            return Json(new HandleResponse<LoginModel>(false, "Username or password is incorrect", null));
        }
        private string BuildTokenERP(TCMUSMT masUser)
        {

            List<Claim> claims = new List<Claim>
            {
                new Claim("user_name", masUser.Name),
                new Claim("user_id", masUser.UserId),

            };

            var extra = new ClaimsIdentity();
            extra.AddClaim(new Claim(ClaimTypes.Name, masUser.Name));
            extra.AddClaims(claims);

            //var claims = new[] { new Claim(ClaimTypes.Name, masUser.USER_NM) }            

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
              _config["Jwt:Issuer"],
              _config["Jwt:Issuer"],
              extra.Claims,
              expires: DateTime.Now.AddDays(7),
              signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        [ApiExplorerSettings(GroupName = "fg_inventory_mobile")]
        [HttpGet(nameof(GetMobileMenu))]
        [AllowAnonymous]
        public async Task<HandleList<ZmMasMobileMenuGetModel>> GetMobileMenu(string userId)
        {
            try
            {
                List<ZmMasMobileMenuGetModel> result = await _service.GetMobileMenu(userId);
                return new HandleList<ZmMasMobileMenuGetModel>(result);
            }
            catch (Exception e)
            {
                return new HandleList<ZmMasMobileMenuGetModel>(false, e.Message);
            }
        }
    }
}
