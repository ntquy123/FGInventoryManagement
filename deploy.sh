#!/bin/bash

# ==============================================
#        PKFGM DEPLOYMENT SCRIPT - V3.0
# ==============================================

# --- CONFIG COLORS ---
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

# --- LOG FUNCTIONS ---
log_info() { echo -e "  ${B_BLUE}[INFO]${NC} $1"; }
log_process() { echo -e "  ${B_CYAN}[RUNNING]${NC} $1..."; }
log_success() { echo -e "  ${B_GREEN}[SUCCESS]${NC} $1"; }
log_error() {
    echo -e "\n${BG_RED}${B_WHITE}  CRITICAL ERROR  ${NC}"
    echo -e "${B_RED}👉 $1${NC}\n"
}

separator() {
    echo -e "${B_BLUE}════════════${B_MAGENTA}════════════${CYAN}════════════${NC}"
}

log_step() {
    separator
    echo -e "${B_MAGENTA}STEP $1:${NC} ${B_WHITE}$2${NC}"
    separator
    echo ""
}

# --- DRAW HEADER : PKFGM GALAXY 3D ---
draw_header() {
    clear
    printf "\n"
    printf "${B_MAGENTA}      ██████╗ ██╗  ██╗███████╗ ██████╗ ███╗   ███╗${NC}\n"
    printf "${B_MAGENTA}      ██╔══██╗██║ ██╔╝██╔════╝██╔════╝ ████╗ ████║${NC}\n"
    printf "${B_BLUE}       ██████╔╝█████╔╝ █████╗  ██║  ███╗██╔████╔██║${NC}\n"
    printf "${B_CYAN}       ██╔═══╝ ██╔═██╗ ██╔══╝  ██║   ██║██║╚██╔╝██║${NC}\n"
    printf "${B_WHITE}      ██║     ██║  ██╗██║     ╚██████╔╝██║ ╚═╝ ██║${NC}\n"
    printf "${B_WHITE}      ╚═╝     ╚═╝  ╚═╝╚═╝      ╚═════╝ ╚═╝     ╚═╝${NC}\n"

    printf "\n"
    printf "${CYAN}      ✦ Galaxy Deployment System ✦${NC}\n"
    printf "${B_MAGENTA}      API • Docker • Nginx • Automation${NC}\n"
    printf "\n"
    sleep 1
}

# ==============================================
#           RUN DEPLOY PROCESS
# ==============================================

draw_header

# --- STEP 1: GIT PULL ---
log_step "1" "Syncing latest source code from Git"
log_process "git pull"

if git_output=$(git pull 2>&1); then
    echo -e "${CYAN}${git_output}${NC}"
    log_success "Source code updated successfully"
else
    echo -e "${RED}${git_output}${NC}"
    log_error "Git pull failed"
    exit 1
fi

# --- STEP 2: DOCKER COMPOSE DOWN ---
log_step "2" "Stopping old containers"
log_process "docker compose down"

if docker compose down; then
    log_success "Old containers stopped"
else
    log_error "Failed to stop containers"
    exit 1
fi

# --- STEP 3: DOCKER COMPOSE UP ---
log_step "3" "Building and starting PKFGM system"
log_process "docker compose up --build -d"

if docker compose up --build -d --remove-orphans; then
    log_success "PKFGM deployed successfully"
else
    log_error "Docker build failed"
    exit 1
fi

# --- FINAL ---
separator
echo -e "\n${B_GREEN}DEPLOYMENT COMPLETED — SYSTEM ONLINE${NC}\n"
separator

log_info "Container status:"
docker compose ps
echo ""
