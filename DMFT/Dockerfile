FROM openjdk:8-jdk-alpine as build
WORKDIR /workspace/app

COPY mvnw .
COPY .mvn .mvn
COPY pom.xml .
COPY src src
COPY .env .env

RUN ./mvnw package -DskipTests
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

EXPOSE 80 433
