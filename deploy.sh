#!/bin/bash

# ==============================================
#    FGInventory DEPLOYMENT SCRIPT - V2.0 (WOW EDITION)
# ==============================================

# --- ADVANCED COLOR & STYLE PALETTE ---
# Standard & Bold Colors
NC='\033[0m' # No Color
BLACK='\033[0;30m'  B_BLACK='\033[1;30m'
RED='\033[0;31m'    B_RED='\033[1;31m'
GREEN='\033[0;32m'  B_GREEN='\033[1;32m'
YELLOW='\033[0;33m' B_YELLOW='\033[1;33m'
BLUE='\033[0;34m'   B_BLUE='\033[1;34m'
PURPLE='\033[0;35m' B_MAGENTA='\033[1;35m'
CYAN='\033[0;36m'   B_CYAN='\033[1;36m'
WHITE='\033[0;37m'  B_WHITE='\033[1;37m'

# Backgrounds (Optional use for extreme emphasis)
BG_RED='\033[41m'
BG_GREEN='\033[42m'

# Styles
BOLD='\033[1m'
UNDERLINE='\033[4m'
BLINK='\033[5m' # Use sparingly!

# --- LOGGING FUNCTIONS WITH ICONS ---
log_info() {
    echo -e "  ${B_BLUE}ℹ️   [INFO]${NC} $1"
}

log_process() {
    echo -e "  ${B_CYAN}⚙️   [EXECUTING]${NC} $1..."
}

log_success() {
    echo -e "  ${B_GREEN}✅  [SUCCESS]${NC} $1"
}

log_error() {
    echo -e "\n${BG_RED}${B_WHITE} 🛑 FATAL ERROR 🛑 ${NC}"
    echo -e "${B_RED}👉 $1${NC}\n"
}

# A fancy separator bar with a gradient effect
separator() {
    echo -e "\n${B_BLUE}════════════════${B_MAGENTA}════════════════${B_BLUE}════════════════${NC}"
}

log_step() {
    separator
    echo -e "${B_MAGENTA}🚀  STEP $1:${NC} ${B_WHITE}$2${NC}"
    separator
    echo ""
}

# --- THE "WOW" HEADER (FGInventory Mobile) ---
draw_header() {
    clear
    echo -e "${B_BLUE}"
    echo "      .╔════════════════════════╗."
    echo "      ║   ${B_CYAN}◉${B_BLUE}  ${B_BLACK}···· ${B_BLUE}  ${B_CYAN}▂▃▅▇█${B_BLUE}   ║"  # Signal/Camera bar
    echo "      ║.────────────────────────.║"
    echo "      ║│                        │║"
    echo "      ║│   ${B_MAGENTA}╔═╗╔═╗${B_CYAN}╦╔╗╔╦  ╦${B_BLUE}    │║"
    echo "      ║│   ${B_MAGENTA}╠═╝║ ╦${B_CYAN}║║║║╚╗╔╝╠═╗${B_BLUE}   │║"
    echo "      ║│   ${B_MAGENTA}╩  ╚═╝${B_CYAN}╩╝╚╝ ╚╝ ╩ ╩${B_BLUE}   │║"
    echo "      ║│                        │║"
    echo "      ║│   ${B_WHITE}MOBILE DEPLOYMENT${B_BLUE}    │║"
    echo "      ║│      ${B_GREEN}STATUS: READY${B_BLUE}     │║"
    echo "      ║│                        │║"
    echo "      ║'────────────────────────'║"
    echo "      ║      ${B_BLACK}[  ${B_CYAN}●${B_BLACK}  ]${B_BLUE}      ║" # Home button
    echo "      '╚════════════════════════╝'"
    echo -e "${NC}"
    echo -e "${B_WHITE}    Starting Automated Sequence...${NC}\n"
    sleep 1
}

# ==============================================
#    MAIN EXECUTION FLOW
# ==============================================

draw_header

# --- STEP 1: GIT PULL ---
log_step "1" "Synchronizing Source Code (Git)"
log_info "Fetching latest changes from remote repository..."

log_process "Running: git pull"
# Capture output and check status
if git_output=$(git pull 2>&1); then
    echo -e "${CYAN}${git_output}${NC}" # Show git output in a subtle color
    log_success "Source code is up-to-date!"
else
    # If it fails, show the output in red
    echo -e "${RED}${git_output}${NC}"
    log_error "Git pull failed! Please check connection or conflicts."
    exit 1
fi


# --- STEP 2: DOCKER COMPOSE DOWN ---
log_step "2" "Tearing Down Existing Infrastructure"
log_info "Stopping currently running containers to ensure clean state."

log_process "Running: docker compose down"
if docker compose down; then
    log_success "Environment successfully stopped and removed."
else
    log_error "Failed to stop existing containers."
    exit 1
fi


# --- STEP 3: DOCKER COMPOSE UP ---
log_step "3" "Building & Launching FGInventory Mobile"
log_info "Rebuilding images and starting services in detached mode."
log_info "Using '--remove-orphans' to clean up stale services."

log_process "Running: docker compose up --build -d"
# Added --remove-orphans for hygiene
if docker compose up --build -d --remove-orphans; then 
    log_success "Containers launched successfully!"
else
    log_error "Docker build or startup failed."
    exit 1
fi


# --- FINALIZATION ---
separator
echo -e "\n${B_GREEN}${BOLD}🎉✨  DEPLOYMENT COMPLETE! SYSTEM IS ONLINE.  ✨🎉${NC}\n"
separator

log_info "Current Container Status:"
# Show status with a slight delay for dramatic effect
sleep 1
docker compose ps
echo ""