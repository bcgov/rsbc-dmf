# RSBC DFM Landing page

Hosts static pages for RSBC-DFM project

Stack:

- Caddy 2

Steps:

Build & Run the Reverse Proxy:
docker build -t apache-reverse-proxy . && docker run -d -p 8080:80 --name apache-container apache-reverse-proxy

Build & Run Caddy: 
docker build -t my-caddy . && docker run -d -p 2015:2015 --name caddy my-caddy

You can then access http://localhost:8080/roadsafetybc/ to simulate the topology of the Apache proxy server

Or access Caddy web server directly: http://localhost:2015/