#FROM registry.redhat.io/rhscl/mongodb-26-rhel7
FROM mongo:4.0.4

# Define mountable directories.
VOLUME ["/data/db"]

# Expose ports.
#   - 27017: process
#   - 28017: http
EXPOSE 27017 28017