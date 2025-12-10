#!/bin/bash

# --- COLOR CONFIGURATION ---
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
CYAN='\033[0;36m'
NC='\033[0m' # No Color

# --- LOGGING FUNCTIONS ---
log_info() {
    echo -e "${CYAN}[INFO]${NC} $1"
}

log_success() {
    echo -e "${GREEN}[SUCCESS]${NC} $1"
}

log_error() {
    echo -e "${RED}[ERROR]${NC} $1"
}

log_step() {
    echo -e "\n${YELLOW}==============================================${NC}"
    echo -e "${YELLOW}➤ STEP $1: $2${NC}"
    echo -e "${YELLOW}==============================================${NC}"
}

# --- START SCRIPT ---
echo -e "${GREEN}"
echo "  ___  ___  ___ _    _____   __"
echo " |   \| __|| _ \ |  / _ \ \ / /"
echo " | |) | _| |  _/ |_| (_) \ V / "
echo " |___/|___||_| |____\___/ |_|  "
echo "     AUTO DEPLOY SCRIPT        "
echo -e "${NC}"

# STEP 1: GIT PULL
log_step "1" "Updating Source Code (Git Pull)"
if git pull; then
    log_success "Git pull completed successfully!"
else
    log_error "Git pull failed! Please check your network connection or merge conflicts."
    exit 1 # Stop script immediately on error
fi

# STEP 2: DOCKER COMPOSE DOWN
log_step "2" "Stopping and Removing Old Containers (Down)"
if docker compose down; then
    log_success "Old containers stopped and removed."
else
    log_error "Error executing 'docker compose down'."
    exit 1
fi

# STEP 3: DOCKER COMPOSE UP
log_step "3" "Building and Starting New Containers (Up)"
# Added --remove-orphans to keep the environment clean
if docker compose up --build -d --remove-orphans; then 
    log_success "New containers started successfully!"
else
    log_error "Error building or starting containers."
    exit 1
fi

# FINISH
echo -e "\n${GREEN}✅  DEPLOYMENT COMPLETED! SYSTEM IS RUNNING.  ✅${NC}\n"

# (Optional) Show current container status
docker compose ps