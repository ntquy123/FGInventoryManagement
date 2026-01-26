#!/usr/bin/env bash
# ==============================================
#              PKFGM DEPLOYMENT - V4.0
#         Production-style (Git + Docker)
# ==============================================

set -Eeuo pipefail
IFS=$'\n\t'

# ---------------------------
# Configuration
# ---------------------------
APP_NAME="${APP_NAME:-PKFGM}"
BRANCH="${BRANCH:-main}"
COMPOSE_CMD="${COMPOSE_CMD:-docker compose}"
ROLLBACK_ON_FAIL="${ROLLBACK_ON_FAIL:-1}"
PROGRESS_SPEED="${PROGRESS_SPEED:-0.02}"   # smaller = faster
TWINKLE_FRAMES="${TWINKLE_FRAMES:-6}"      # galaxy animation frames

# ---------------------------
# Colors (ANSI)
# ---------------------------
NC='\033[0m'
B_BLACK='\033[1;30m'
RED='\033[0;31m'    B_RED='\033[1;31m'
GREEN='\033[0;32m'  B_GREEN='\033[1;32m'
B_YELLOW='\033[1;33m'
BLUE='\033[0;34m'   B_BLUE='\033[1;34m'
MAGENTA='\033[0;35m' B_MAGENTA='\033[1;35m'
CYAN='\033[0;36m'   B_CYAN='\033[1;36m'
B_WHITE='\033[1;37m'
BG_RED='\033[41m'

# ---------------------------
# Logging
# ---------------------------
timestamp() { date +"%Y-%m-%d %H:%M:%S"; }

log_info()    { echo -e "[$(timestamp)] ${B_BLUE}[INFO]${NC}    $*"; }
log_warn()    { echo -e "[$(timestamp)] ${B_YELLOW}[WARN]${NC}    $*"; }
log_process() { echo -e "[$(timestamp)] ${B_CYAN}[RUN]${NC}     $*"; }
log_success() { echo -e "[$(timestamp)] ${B_GREEN}[OK]${NC}      $*"; }
log_error() {
  echo -e "\n${BG_RED}${B_WHITE}  ERROR  ${NC}"
  echo -e "${B_RED}[$(timestamp)] $*${NC}\n"
}

separator() {
  echo -e "${B_BLUE}════════════${B_MAGENTA}════════════${CYAN}════════════${NC}"
}

step() {
  separator
  echo -e "${B_MAGENTA}STEP $1${NC} ${B_WHITE}$2${NC}"
  separator
}

# ---------------------------
# Helpers
# ---------------------------
require_cmd() {
  command -v "$1" >/dev/null 2>&1 || {
    log_error "Missing required command: $1"
    exit 1
  }
}

is_git_repo() {
  git rev-parse --is-inside-work-tree >/dev/null 2>&1
}

fake_progress() {
  # usage: fake_progress "Message"
  local msg="$1"
  local width=28
  local i
  printf "%s " "$msg"
  printf "["
  for ((i=0; i<width; i++)); do
    printf " "
  done
  printf "]"
  printf "\r%s [" "$msg"
  for ((i=0; i<=width; i++)); do
    printf "#"
    sleep "$PROGRESS_SPEED"
  done
  printf "]\n"
}

twinkle_galaxy() {
  # small animated header; short frames to avoid noisy logs
  local i
  clear
  for ((i=1; i<=TWINKLE_FRAMES; i++)); do
    clear
    printf "\n"
    printf "${B_MAGENTA}      ██████╗ ██╗  ██╗███████╗ ██████╗ ███╗   ███╗${NC}\n"
    printf "${B_MAGENTA}      ██╔══██╗██║ ██╔╝██╔════╝██╔════╝ ████╗ ████║${NC}\n"
    printf "${B_BLUE}       ██████╔╝█████╔╝ █████╗  ██║  ███╗██╔████╔██║${NC}\n"
    printf "${B_CYAN}       ██╔═══╝ ██╔═██╗ ██╔══╝  ██║   ██║██║╚██╔╝██║${NC}\n"
    printf "${B_WHITE}      ██║     ██║  ██╗██║     ╚██████╔╝██║ ╚═╝ ██║${NC}\n"
    printf "${B_WHITE}      ╚═╝     ╚═╝  ╚═╝╚═╝      ╚═════╝ ╚═╝     ╚═╝${NC}\n"
    printf "\n"

    # Twinkle line changes by frame
    case "$i" in
      1) printf "${CYAN}      ✦   ·      ✧     ·   ✦     ·      ✧${NC}\n" ;;
      2) printf "${CYAN}      ·   ✦      ·     ✧   ·     ✦      ·${NC}\n" ;;
      3) printf "${CYAN}      ✧   ·   ✦        ·   ✧        ✦   ·${NC}\n" ;;
      4) printf "${CYAN}      ·      ✧   ·   ✦     ·   ✧      ·  ${NC}\n" ;;
      5) printf "${CYAN}      ✦     ·   ✧     ·      ✦     ·   ✧${NC}\n" ;;
      *) printf "${CYAN}      ·   ✦      ·     ✧   ·     ✦      ·${NC}\n" ;;
    esac

    printf "${B_MAGENTA}      Galaxy Deployment System${NC}\n"
    printf "${B_WHITE}      API • Docker • Nginx • Automation${NC}\n"
    printf "\n"
    sleep 0.15
  done
}

print_environment() {
  step "0" "Environment checks (Linux + Docker)"

  # Linux / OS info
  if [ -f /etc/os-release ]; then
    # shellcheck disable=SC1091
    . /etc/os-release
    log_info "OS: ${PRETTY_NAME:-Unknown}"
  else
    log_warn "Cannot read /etc/os-release (OS info unknown)"
  fi

  log_info "Kernel: $(uname -r)"

  # Docker / Compose / Git versions
  require_cmd git
  require_cmd docker

  log_info "Git: $(git --version | awk '{print $3}')"
  log_info "Docker: $(docker --version | sed 's/,//g')"

  if docker compose version >/dev/null 2>&1; then
    log_info "Docker Compose: $(docker compose version | head -n 1)"
  elif command -v docker-compose >/dev/null 2>&1; then
    log_info "Docker Compose: $(docker-compose --version)"
    COMPOSE_CMD="docker-compose"
  else
    log_error "Docker Compose not found (docker compose / docker-compose)."
    exit 1
  fi

  # Docker daemon access
  if ! docker info >/dev/null 2>&1; then
    log_error "Docker daemon not accessible. Ensure Docker is running and user has permission."
    log_info "Tip: sudo systemctl start docker  (or add user to docker group and re-login)"
    exit 1
  fi

  # Basic repo validation
  if ! is_git_repo; then
    log_error "Current directory is not a Git repository. cd into your project folder first."
    exit 1
  fi

  # Compose file check
  if [ ! -f docker-compose.yml ] && [ ! -f compose.yml ] && [ ! -f compose.yaml ] && [ ! -f docker-compose.yaml ]; then
    log_warn "No compose file detected in current directory. Ensure docker-compose.yml (or compose.yml) exists."
  fi

  log_success "Environment checks passed"
}

# ---------------------------
# Rollback logic
# ---------------------------
PREV_COMMIT=""
NEW_COMMIT=""
DEPLOY_PHASE="init"

rollback() {
  # Called only when failure happens and rollback is enabled.
  if [ "${ROLLBACK_ON_FAIL}" != "1" ]; then
    log_warn "Rollback disabled; skipping."
    return 0
  fi

  if [ -z "${PREV_COMMIT}" ]; then
    log_warn "No previous commit captured; rollback not possible."
    return 0
  fi

  step "RB" "Rollback to previous stable state"
  log_warn "Deploy failed during phase: ${DEPLOY_PHASE}"
  log_warn "Rolling back Git to commit: ${PREV_COMMIT}"

  # Best-effort rollback; do not crash rollback itself
  set +e
  ${COMPOSE_CMD} down >/dev/null 2>&1

  git reset --hard "${PREV_COMMIT}" >/dev/null 2>&1
  if [ $? -ne 0 ]; then
    log_error "Rollback failed: could not reset Git to ${PREV_COMMIT}"
    set -e
    return 1
  fi

  log_process "Redeploying previous version (compose up -d --build)"
  ${COMPOSE_CMD} up -d --build --remove-orphans
  if [ $? -eq 0 ]; then
    log_success "Rollback completed successfully"
    set -e
    return 0
  else
    log_error "Rollback redeploy failed. Manual intervention required."
    set -e
    return 1
  fi
}

on_error() {
  local exit_code=$?
  log_error "Script aborted (exit code: ${exit_code})"
  rollback || true
  exit "${exit_code}"
}
trap on_error ERR

# ---------------------------
# Main deploy steps
# ---------------------------
main() {
  twinkle_galaxy
  print_environment

  step "1" "Sync latest source code from Git"
  DEPLOY_PHASE="git-sync"

  PREV_COMMIT="$(git rev-parse HEAD)"
  log_info "Current commit: ${PREV_COMMIT}"

  # Ensure correct branch
  current_branch="$(git rev-parse --abbrev-ref HEAD)"
  if [ "${current_branch}" != "${BRANCH}" ]; then
    log_warn "Current branch is '${current_branch}'. Switching to '${BRANCH}'."
    git checkout "${BRANCH}"
  fi

  fake_progress "Pulling latest code"
  log_process "git pull"
  git_output="$(git pull 2>&1)"
  echo -e "${CYAN}${git_output}${NC}"

  NEW_COMMIT="$(git rev-parse HEAD)"
  if [ "${NEW_COMMIT}" = "${PREV_COMMIT}" ]; then
    log_info "No new commit detected (already up to date)."
  else
    log_success "Updated commit: ${NEW_COMMIT}"
  fi

  step "2" "Stopping old containers"
  DEPLOY_PHASE="compose-down"
  fake_progress "Stopping services"
  ${COMPOSE_CMD} down
  log_success "Old containers stopped"

  step "3" "Build and start services"
  DEPLOY_PHASE="compose-up"
  fake_progress "Building & starting"
  ${COMPOSE_CMD} up -d --build --remove-orphans
  log_success "Services started successfully"

  step "4" "Post-deploy status"
  DEPLOY_PHASE="post-check"

  log_info "Container status:"
  ${COMPOSE_CMD} ps

  separator
  echo -e "${B_GREEN}${APP_NAME} DEPLOYMENT COMPLETED — SYSTEM ONLINE${NC}"
  separator
}

main "$@"
