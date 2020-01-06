# Jira

WIP Task manager for teams, implemented using dotnet core MVC for an University class.

For login you can use the admin user created at [startup](./appsettings.Development.json) or register a new one.

## Docker support

You can run the app using docker:

### Clone the repo and build it yourself

```shell
git clone https://github.com/iugastefan/Jira.git
cd Jira
docker build -t jira .
docker run -d -p 8080:80 --name Jira jira
```
### Or

```shell
docker run -d -p 8080:80 --name Jira iugastefan/jira:1.0
```
Then access http://localhost:8080/
