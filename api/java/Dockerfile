FROM maven:3-jdk-8-openj9 as build
WORKDIR /workspace/app

COPY mvnw .
COPY .mvn .mvn
COPY pom.xml .
COPY src src
COPY .env .env

ENV SIMULATE_MONGO true
RUN mvn clean install 
ENV SIMULATE_MONGO false

# For now will start in the copied folder set below rather than the jar
RUN mkdir -p target/dependency && (cd target/dependency; jar -xf ../*.jar)

FROM openjdk:8-jdk-alpine
#FROM registry.redhat.io/openjdk/openjdk-8-rhel8 AS runtime

VOLUME /tmp
ARG DEPENDENCY=/workspace/app/target/dependency
COPY --from=build ${DEPENDENCY}/BOOT-INF/lib /app/lib
COPY --from=build ${DEPENDENCY}/META-INF /app/META-INF
COPY --from=build ${DEPENDENCY}/BOOT-INF/classes /app
COPY --from=build ${DEPENDENCY}/org /app/org

WORKDIR app

CMD ["java", "-cp", "./:lib/*", "org.springframework.boot.loader.JarLauncher"]

# By default Sring Boot runs on 8080
EXPOSE 8080 433
