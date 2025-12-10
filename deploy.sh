#!/bin/bash

# ==============================================
#    FGInventory DEPLOYMENT SCRIPT - V2.1 (STABLE WOW)
# ==============================================

# --- CẤU HÌNH MÀU SẮC ---
NC='\033[0m' 
B_BLACK='\033[1;30m'
RED='\033[0;31m'    B_RED='\033[1;31m'
GREEN='\033[0;32m'  B_GREEN='\033[1;32m'
B_YELLOW='\033[1;33m'
BLUE='\033[0;34m'   B_BLUE='\033[1;34m'
B_MAGENTA='\033[1;35m'
CYAN='\033[0;36m'   B_CYAN='\033[1;36m'
B_WHITE='\033[1;37m'
BG_RED='\033[41m'

# --- HÀM LOG ---
log_info() { echo -e "  ${B_BLUE}ℹ️   [INFO]${NC} $1"; }
log_process() { echo -e "  ${B_CYAN}⚙️   [ĐANG CHẠY]${NC} $1..."; }
log_success() { echo -e "  ${B_GREEN}✅  [THÀNH CÔNG]${NC} $1"; }
log_error() { 
    echo -e "\n${BG_RED}${B_WHITE} 🛑 LỖI NGHIÊM TRỌNG 🛑 ${NC}"
    echo -e "${B_RED}👉 $1${NC}\n"
}

separator() { echo -e "\n${B_BLUE}════════════════${B_MAGENTA}════════════════${B_BLUE}════════════════${NC}"; }

log_step() {
    separator
    echo -e "${B_MAGENTA}🚀  BƯỚC $1:${NC} ${B_WHITE}$2${NC}"
    separator
    echo ""
}

# --- VẼ HÌNH MOBILE (Dùng printf cho chuẩn) ---
draw_header() {
    clear
    printf "${B_BLUE}"
    printf "      .╔════════════════════════╗.\n"
    printf "      ║   ${B_CYAN}◉${B_BLUE}  ${B_BLACK}···· ${B_BLUE}  ${B_CYAN}▂▃▅▇█${B_BLUE}   ║\n"
    printf "      ║.────────────────────────.║\n"
    printf "      ║│                        │║\n"
    printf "      ║│   ${B_MAGENTA}╔═╗╔═╗${B_CYAN}╦╔╗╔╦  ╦${B_BLUE}    │║\n"
    printf "      ║│   ${B_MAGENTA}╠═╝║ ╦${B_CYAN}║║║║╚╗╔╝╠═╗${B_BLUE}   │║\n"
    printf "      ║│   ${B_MAGENTA}╩  ╚═╝${B_CYAN}╩╝╚╝ ╚╝ ╩ ╩${B_BLUE}   │║\n"
    printf "      ║│                        │║\n"
    printf "      ║│   ${B_WHITE}MOBILE DEPLOYMENT${B_BLUE}    │║\n"
    printf "      ║│      ${B_GREEN}STATUS: READY${B_BLUE}     │║\n"
    printf "      ║│                        │║\n"
    printf "      ║'────────────────────────'║\n"
    printf "      ║      ${B_BLACK}[  ${B_CYAN}●${B_BLACK}  ]${B_BLUE}      ║\n"
    printf "      '╚════════════════════════╝'\n"
    printf "${NC}\n"
    echo -e "${B_WHITE}    Đang khởi động quy trình tự động...${NC}\n"
    sleep 1
}

# ==============================================
#    CHẠY CHƯƠNG TRÌNH
# ==============================================

draw_header

# --- BƯỚC 1: GIT PULL ---
log_step "1" "Đồng bộ Source Code (Git)"
log_process "Đang chạy: git pull"

if git_output=$(git pull 2>&1); then
    echo -e "${CYAN}${git_output}${NC}"
    log_success "Source code đã mới nhất!"
else
    echo -e "${RED}${git_output}${NC}"
    log_error "Git pull thất bại! Kiểm tra lại conflict code."
    exit 1
fi

# --- BƯỚC 2: DOCKER COMPOSE DOWN ---
log_step "2" "Dọn dẹp container cũ"
log_process "Đang chạy: docker compose down"

if docker compose down; then
    log_success "Đã dừng và xóa container cũ."
else
    log_error "Không thể dừng container."
    exit 1
fi

# --- BƯỚC 3: DOCKER COMPOSE UP ---
log_step "3" "Build & Khởi chạy FGInventory"
log_process "Đang chạy: docker compose up --build -d"

if docker compose up --build -d --remove-orphans; then 
    log_success "Hệ thống khởi động thành công!"
else
    log_error "Lỗi khi build docker."
    exit 1
fi

# --- KẾT THÚC ---
separator
echo -e "\n${B_GREEN}🎉✨  DEPLOY HOÀN TẤT! APP ĐÃ ONLINE.  ✨🎉${NC}\n"
separator

log_info "Trạng thái container hiện tại:"
sleep 1
docker compose ps
echo ""