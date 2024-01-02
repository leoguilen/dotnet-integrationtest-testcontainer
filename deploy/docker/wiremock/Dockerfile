FROM wiremock/wiremock:latest
COPY ./stubs /home/wiremock
ENTRYPOINT ["/docker-entrypoint.sh", "--global-response-templating", "--disable-gzip", "--verbose"]