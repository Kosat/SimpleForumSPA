FROM docker.elastic.co/kibana/kibana-oss:6.2.4
COPY ./config/kibana.yml /usr/share/kibana/config/kibana.yml

# RUN kibana-plugin install https://github.com/sivasamyk/logtrail/releases/download/v0.1.27/logtrail-6.2.4-0.1.27.zip
# Because of this bug: https://github.com/elastic/kibana/issues/19678
# Install plugin manually
COPY ./plugins/logtrail/ /usr/share/kibana/plugins/logtrail/
