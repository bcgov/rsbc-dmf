FROM httpd:latest

# Enable mod_proxy
RUN sed -i '/#LoadModule proxy_module/s/#//g' /usr/local/apache2/conf/httpd.conf
RUN sed -i '/#LoadModule proxy_http_module/s/#//g' /usr/local/apache2/conf/httpd.conf

# Configure reverse proxy
RUN echo 'ProxyPass "/roadsafetybc/" "http://host.docker.internal:2015/"' >> /usr/local/apache2/conf/httpd.conf
RUN echo 'ProxyPassReverse "/roadsafetybc/" "http://host.docker.internal:2015/"' >> /usr/local/apache2/conf/httpd.conf