CB Banhammer web conversion.

## Code structure

This is a simple project, it only has 4 pieces right now

- `wwwroot` - these files are served directly by the webserver as static files.
- `DataClient.cs` and `DataClasses.cs` are responsible for the data layer.
- `Program.cs` is the main function, which is setting up and running the ASP.NET MVC web server. It also has all the controller code for AJAX requests.
- `pages` - These are the Razor C# pages.

### Data layer
I am using `Dapper` as a lightweight layer that converts the native ADO.NET API to strongly-typed classes, as defined in `DataClasses.cs` . 

Each function in the `DataClient.cs` is responsible for one 'data action' (typically, one SQL query). 

### Razor pages
Each page is a pair of files: 
- `page_name.cshtml` - these are the actual HTML pages, which are augmented with the 'model' information, that is taken from the corresponding `PageNameModel.cs` files. 
- `PageNameModel.cs` - this is the server-side C# code that runs for every request. Typically, the `OnGet()` method the model class is calling the `DataClient` to fetch the data, and then stores it in some public property, for the `cshtml` to render.
