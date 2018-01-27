# Project Bastion: Barbican

A secure proxy to allow authorised access to internal services without exposing them directly.


## Routes

```
/proxy/<scheme>/<host_and_port>/<path> ==> <scheme>://<host_and_port>/<path>
```

e.g. 
```
/proxy/https/www.bbc.co.uk/news ==> http://www.bbc.co.uk/news
```
