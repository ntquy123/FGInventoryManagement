#!/usr/bin/env bash
# ==============================================
#        PKFGM DEPLOYMENT SCRIPT - V4.2
#        Pull-only / No checkout / No commit
# ==============================================

set -Eeuo pipefail
IFS=$'\n\t'

# ---------------------------
# Config
# ---------------------------
APP_NAME="PKFGM"
COMPOSE_CMD="docker compose"
PROGRESS_SPEED=0.02

# ---------------------------
# Colors
# ---------------------------
NC='\033[0m'
RED='\033[0;31m'    B_RED='\033[1;31m'
GREEN='\033[0;32m'  B_GREEN='\033[1;32m'
BLUE='\033[0;34m'   B_BLUE='\033[1;34m'
MAGENTA='\033[0;35m' B_MAGENTA='\033[1;35m'
CYAN='\033[0;36m'   B_CYAN='\033[1;36m'
B_WHITE='\033[1;37m'
BG_RED='\033[41m'

# ---------------------------
# Logging
# ---------------------------
 ts() { date +"%Y-%m-%d %H:%M:%S"; }

log_info()    { echo -e "[$(ts)] ${B_BLUE}[INFO]${NC} $*"; }
log_warn()    { echo -e "[$(ts)] ${B_MAGENTA}[WARN]${NC} $*"; }
log_run()     { echo -e "[$(ts)] ${B_CYAN}[RUN]${NC}  $*"; }
log_ok()      { echo -e "[$(ts)] ${B_GREEN}[OK]${NC}   $*"; }

log_error() {
  echo -e "\n${BG_RED}${B_WHITE} ERROR ${NC}"
  echo -e "${B_RED}[$(ts)] $*${NC}\n"
}


sep() { echo -e "${B_BLUE}════════════${B_MAGENTA}════════════${CYAN}════════════${NC}"; }

# ---------------------------
# Helpers
# ---------------------------
require() {
  command -v "$1" >/dev/null 2>&1 || {
    log_error "Missing required command: $1"
    exit 1
  }
}

fake_progress() {
  local msg="$1"
  local w=26
  printf "%s [" "$msg"
  for ((i=0;i<w;i++)); do printf " "; done
  printf "]\r%s [" "$msg"
  for ((i=0;i<=w;i++)); do
    printf "#"
    sleep "$PROGRESS_SPEED"
  done
  printf "]\n"
}

# ---------------------------
# Header
# ---------------------------
draw_header() {
  clear
  echo -e "${B_MAGENTA}
 ██████╗ ██╗  ██╗███████╗ ██████╗ ███╗   ███╗
 ██╔══██╗██║ ██╔╝██╔════╝██╔════╝ ████╗ ████║
 ██████╔╝█████╔╝ █████╗  ██║  ███╗██╔████╔██║
 ██╔═══╝ ██╔═██╗ ██╔══╝  ██║   ██║██║╚██╔╝██║
 ██║     ██║  ██╗██║     ╚██████╔╝██║ ╚═╝ ██║
 ╚═╝     ╚═╝  ╚═╝╚═╝      ╚═════╝ ╚═╝     ╚═╝
${NC}"
  echo -e "${CYAN} Galaxy Deployment System${NC}"
  echo -e "${B_WHITE} API • Docker • Nginx${NC}\n"
}

# ---------------------------
# Rollback Docker only
# ---------------------------
rollback_docker() {
  log_warn "Rolling back Docker containers"
  ${COMPOSE_CMD} down || true
  ${COMPOSE_CMD} up -d || true
}

trap 'log_error "Deploy failed"; rollback_docker' ERR

# ---------------------------
# Main
# ---------------------------
main() {
  draw_header

  require git
  require docker

  # Environment info
  sep
  log_info "OS: $(grep PRETTY_NAME /etc/os-release | cut -d= -f2 | tr -d '\"')"
  log_info "Kernel: $(uname -r)"
  log_info "Git: $(git --version)"
  log_info "Docker: $(docker --version)"
  log_info "Branch: $(git rev-parse --abbrev-ref HEAD)"
  sep

  # STEP 1: Git pull (current branch only)
  sep
  log_info "STEP 1: Pull latest code (current branch only)"
  fake_progress "git pull"

  if ! git pull; then
    log_error "git pull failed. Local changes detected or merge conflict."
    exit 1
  fi

  log_ok "Git pull completed"

  # STEP 2: Docker down
  sep
  log_info "STEP 2: Stop old containers"
  fake_progress "docker compose down"
  ${COMPOSE_CMD} down
  log_ok "Containers stopped"

  # STEP 3: Docker build & up
  sep
  log_info "STEP 3: Build & deploy"
  fake_progress "docker compose up"
  ${COMPOSE_CMD} up -d --build --remove-orphans
  log_ok "Deployment successful"

  # Final
  sep
  log_ok "${APP_NAME} IS ONLINE"
  ${COMPOSE_CMD} ps
  sep
}

main
