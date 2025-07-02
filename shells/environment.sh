#!/bin/bash

# define global install path
GLOBAL_INSTALL_PATH=$(pwd)
# export envir
export GLOBAL_INSTALL_PATH

# verify if the export was successful
echo "The current working directory has been exported as the global variable GLOBAL_INSTALL_PATH: $GLOBAL_INSTALL_PATH"