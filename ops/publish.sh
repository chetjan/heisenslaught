#!/bin/bash

set -ex

dir="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"

git pull
cd "$dir/../src/Heisenslaught"
gulp="./node_modules/.bin/gulp"
$gulp clean
$gulp
cd ../..
dotnet restore
dotnet publish

