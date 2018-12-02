# My OVH zone import

## Summary

This project provides a command to update your DNS zone, by OVH.

Using a command, written in C (ifaddr), that assuming a GNU/Linux operating system,
this app will update you DNS zone from OVH services, using 
their Web API, a some configuration.

## Configuration

You'll need for:

* your [OVH Application](https://eu.api.ovh.com/createApp/),
* in order for you to get your private API key, secret,
   and consumer key, by invoking sub-commands `init` and `validate` of `myovh-zone` :

## Execution

```
cd myovh-zone
dotnet run init 
dotnet run validate
dotnet run showconfig
dotnet run zoneupload
```

