# TesseractOCRWebApi
OCR API com Tesseract encapsulada em uma Web API .NET Core


### How to deploy

``` yml
version: '3.4'

services:
...

  tesseract:
    image: ghcr.io/luizcarlosfaria/tesseractocrwebapi/tesseract-ocr-aspnet-webapi:2.1.1
    volumes:
      - /<choose_any_path>/:/<choose_any_path>
    networks:
      - tesseract_net
    
...

networks:
  tesseract_net:
    driver: <overlay|bridge>
```` 

### How to Use

Send a **POST** to `http://tesseract:80/Tesseract/ocr-by-filepath` as **FORM-DATA** with **fileName** parameter as a path of image (on container).

Send a **POST** to `http://tesseract:80/Tesseract/ocr-by-upload` as **multipart/form-data** with **file** as file (upload)



