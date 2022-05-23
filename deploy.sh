#!/bin/bash
set -e
set -o pipefail

Tag=$1
if [ -z "$1" ]
  then
    echo "No tag specified" 1>&2
    exit 1
fi

docker stop nalogica && docker rm nalogica
docker create \
  --name=nalogica \
  --hostname nalogica \
  --net=gunda \
  --user=1000:1000 \
  -v /opt/appdata/ciklonalozi/:/app/data/ \
  -e TZ=Europe/Zagreb \
  -e Auth__GitHubId="ID" \
  -e Auth__GitHubSecret="SECRET" \
  -e Auth__GitHubUsers__0="USER_0" \
  -e Auth__GitHubUsers__1="USER_1" \
  -e LOCALE=hr \
  --restart unless-stopped \
  ghcr.io/prokleta-kuja/ciklonalozi:$Tag
docker start nalogica
