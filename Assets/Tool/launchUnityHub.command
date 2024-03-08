#!/bin/bash
export HTTP_PROXY=http://127.0.0.1:9527
export HTTPS_PROXY=http://127.0.0.1:9527
nohup "/Applications/Unity Hub.app/Contents/MacOS/Unity Hub" &>/dev/null &
