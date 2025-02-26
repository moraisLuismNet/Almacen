## Almacen
ASP.NET Core Web API Almacen

![Almacen](img/1.png)
![Almacen](img/2.png)


## Program
``` 
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AlmacenContext>(options =>
    options.UseSqlServer(connectionString)
);
``` 

## appsetting.Development.json
``` 
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=*;Initial Catalog=Almacen;Integrated Security=True;Encrypt=False"
  }
}
``` 
