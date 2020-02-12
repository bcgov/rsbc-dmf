FROM maven:3-jdk-8-openj9 AS build-env

# Copy source files.
COPY ./src /app/src
COPY pom.xml /app
WORKDIR /app

# Compile the application.
RUN mvn -f pom.xml -DskipTests clean package

# Build runtime image
FROM registry.redhat.io/openjdk/openjdk-8-rhel8 AS runtime

# Copy compiled assemblies into image.
COPY --from=build-env /app/target/*.jar /app/app.jar
WORKDIR /app

ENTRYPOINT ["java","-jar","app.jar"]

# CMD tail -f /dev/null

EXPOSE 8080
