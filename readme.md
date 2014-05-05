# Database Helpers Proof Of Concept

> **NOTE: USE AT YOUR OWN RISK**

## Introduction

One of the greatest features of [RavenDB](http://ravendb.net) is that it ships with an in-memory version of the database engine. This is great for unit/integration tests. Relational Databases struggle with this and normally are abstracted away using stub and mocking techniques.

[LocalDB](http://blogs.msdn.com/b/sqlexpress/archive/2011/07/12/introducing-localdb-a-better-sql-express.aspx) is a new member of the SQL Server family that lets you utilize the SQL Server engine while maintaining the file location of the database. This allows you to ship relatively small databases with apps and connect them to the engine later.

## Idea

Wouldn't it be cool if I could create the database on the fly for each of my test fixtures or individiual tests? **YES, YES IT WOULD!**.

The code you will find in this repository does just that. Take a look at this example. **The example below uses [NPoco](http://www.nuget.org/packages/NPoco/2.4.66-beta) with hard coded SQL. This is just to get the point across. The using statement with LocalDB is the important part.**

```
[Fact]
public void Can_create_and_manipulate_sql_database()
{
    using (var config = new LocalDb("test_database"))
    {
        var database = new Database(config.OpenConnection());

        database.Execute(
        @"CREATE TABLE Person
        (
            Id int IDENTITY PRIMARY KEY,
            Name varchar(60) NULL,
        );");

        database.Insert(new Person { Id = 1, Name = "Khalid Abuhakmeh" });
        var result = database.Query<Person>("select * from Person");
        Assert.True(result.Any());
    }
}
```

What happens here?

1. A database called "test_database" is created and attached to sql server.
2. You are presented with a config class that has information regarding the new database.
3. You can open up a connection to the new database
4. Run your test code or migrations
5. Make your assertion
6. The scope is closed, all connections are shut down, and the database deleted.

## Drawbacks

This is obviously really slow and prone to engine issues like file locks and permission problems. 

## Have Fun

The MIT License (MIT)

Copyright (c) 2014 Khalid Abuhakmeh

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.