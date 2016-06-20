# RavenDBEmbeddedTools
A wrapper that allows you to easily use a RavenDB Embedded NoSQL database : Add, Update, Delete and Query without any configuration.

# Install
```Install-Package RavenDBEmbeddedTools```

# How to use easily
Assuming the following class:
```c#
public class Person
{
    public string Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
}
```

## Insert
```c#
var person = new Person() { FirstName = "Peter", LastName = "Smith" };
RavenDBGenericCommands<Person>.Insert(person);
```

## Update
```c#
RavenDBGenericCommands<Person>.Update(myInstanceOfPerson);
```

## Delete
```c#
RavenDBGenericCommands<Person>.Delete(myInstanceOfPerson.Id);
```

## GetAll
```c#
var persons = RavenDBGenericCommands<Person>.GetAll();
```

## GetById
```c#
var person = RavenDBGenericCommands<Person>.GetById("people/1");
```

## GetByFunc
```c#
Func<Person, bool> searchFunc = (x) => x.LastName.Contains("Smith");
var persons = RavenDBGenericCommands<Person>.GetByFunc(searchFunc);
```

## Count
```c#
var personCount = RavenDBGenericCommands<Person>.Count();
```
