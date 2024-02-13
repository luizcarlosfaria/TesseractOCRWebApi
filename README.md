# TesseractOCRWebApi

Tesseract OCR as a Web API.

**Read to Use** - Just execute a docker run or docker compose up.

**Full Source Available** - Go to [github.com/luizcarlosfaria/TesseractOCRWebApi](https://github.com/luizcarlosfaria/TesseractOCRWebApi) and see all details about this project.

### How to deploy

```yml
version: '3.4'

services:
...

  ocr:
    image: ghcr.io/luizcarlosfaria/tesseractocrwebapi/tesseract-ocr-aspnet-webapi:2.2.0
    ports:
    - "8080:8080"
    volumes:
    - ./ocr/tests:/data
    networks:
    - ocr_net

...

networks:
  ocr_net:
    driver: <overlay|bridge>
```

## How to Use


### Upload 
Send a **POST** to `http://tesseract:8080/tesseract/ocr-by-upload` as **multipart/form-data** with **file** as file (upload)

```
curl --location 'http://localhost:8080/tesseract/ocr-by-upload' \
--form 'file=@"/C:/.../.../your-image.png"'
```


### Shared Folder

Send a **POST** to `http://tesseract:8080/tesseract/ocr-by-filepath` as **FORM-DATA** with **fileName** parameter as a path of image (on container).

```
curl --location 'http://localhost:8080/tesseract/ocr-by-filepath' \
--form 'fileName="/data/1.jpg"'
```


## Security Considerations

For security reasons, only `/tmp/` or `/data/` directories (and children) are accepted as source image directories.

## Requirements

This project was designed to run as a Linux Container (using docker, podman, kubernetes, or Containers as a Services platforms).

If you have docker on local machine (windows or linux), clone this repo and execute `docker-compose up --build`. 

If you are on Windows and you don't have docker, sorry, you can't run this project.
