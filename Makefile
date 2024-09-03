all: ss ws

sb: #server build
	dotnet build ShopAdminTool.Server/ShopAdminTool.Server.sln

st: #server test
	dotnet test ShopAdminTool.Server/ShopAdminTool.Server.sln

ss: #server start
	dotnet run --project ShopAdminTool.Server/ShopAdminTool.Api/ShopAdminTool.Api.csproj


ct: #client test
	npm test --prefix shop-admin-tool.web

ctc: #client test coverage
	npm test --prefix shop-admin-tool.web -- --coverage .

cs: #client start
	npm start --prefix shop-admin-tool.web

du: #docker up
	docker-compose rm -f
	docker-compose pull
	docker-compose -f docker-compose.yml up --build -d
	
dd: #docker down
	docker-compose -f docker-compose.yml down