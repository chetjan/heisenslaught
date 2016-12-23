#!/bin/bash

set -ex

dir="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"

git pull
cd "${dir}/../src/HeisenslaughtUI"
npm install
./node_modules/.bin/ng build --prod
cd ../..
dotnet restore
dotnet publish

