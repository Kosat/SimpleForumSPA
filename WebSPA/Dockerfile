FROM microsoft/dotnet:2.1-sdk AS build
EXPOSE 5001
ENV ASPNETCORE_URLS http://+:5001

# set up node
ENV NODE_VERSION 8.9.4
ENV NODE_DOWNLOAD_SHA 21fb4690e349f82d708ae766def01d7fec1b085ce1f5ab30d9bda8ee126ca8fc
RUN curl -SL "https://nodejs.org/dist/v${NODE_VERSION}/node-v${NODE_VERSION}-linux-x64.tar.gz" --output nodejs.tar.gz \
    && echo "$NODE_DOWNLOAD_SHA nodejs.tar.gz" | sha256sum -c - \
    && tar -xzf "nodejs.tar.gz" -C /usr/local --strip-components=1 \
    && rm nodejs.tar.gz \
    && ln -s /usr/local/bin/node /usr/local/bin/nodejs
# end setting up NodeJS 

WORKDIR /src
COPY WebSPA.sln ./
COPY WebSPA/WebSPA.csproj WebSPA/
RUN dotnet restore WebSPA -nowarn:msb3202,nu1503

COPY . .
WORKDIR /src/WebSPA/ClientApp
RUN npm install
# A WORKAROUND for angular2-logger compilation problem
RUN rm ./node_modules/angular2-logger/app/core/*.ts 
# END WORKAROUND

WORKDIR /src/WebSPA
ENTRYPOINT ["dotnet", "run"]
