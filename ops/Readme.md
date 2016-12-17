## Production Release
1. Login as a sudo user
    1. `sudo systemctl stop heisenslaught.service`
1. Login as the heisenslaught user
    1. `~/ops/publish.sh`
1. Login as a sudo user
    1. `sudo systemctl start heisenslaught.service`

## Production Server Setup
1. Add heisenslaught user: `sudo useradd -m heisenslaught`
1. Follow the Linux Local Setup in this [repo](https://github.com/chetjan/heisenslaught)
    1. Note Angular needs about 1GB RAM to compile the front end, add 512MB of swapspace with swappiness=1 if on a t2.nano ([reference](https://www.digitalocean.com/community/tutorials/how-to-add-swap-space-on-ubuntu-16-04)).
1. Setup NGINX in front of Kestrel as described [here](https://docs.microsoft.com/en-us/aspnet/core/publishing/linuxproduction)
    1. Use the files in this directory where appropriate. There are a few small differences with the guide above.
