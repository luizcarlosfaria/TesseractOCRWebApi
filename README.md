# TesseractOCRWebApi

Tesseract OCR as a Web API.

**Read to Use** - Just execute a docker run or docker compose up.

**Full Source Available** - Go to [github.com/luizcarlosfaria/TesseractOCRWebApi](https://github.com/luizcarlosfaria/TesseractOCRWebApi) and see all details about this project.

## How to deploy

```yml
version: '3.4'

services:

  ocr:
    image: ghcr.io/luizcarlosfaria/tesseractocrwebapi/tesseract-ocr-aspnet-webapi:2.3.0
    ports:
      - "8080:8080"
    volumes:
      - ./ocr/tests:/data
    networks:
      - ocr_net

# other services ...

networks:
  ocr_net:
    driver: <overlay|bridge>
```

## How to Use


### Upload
Send a **POST** to `http://tesseract:8080/tesseract/ocr-by-upload` as **multipart/form-data** with **file** as file (upload)

```shell
curl --location 'http://localhost:8080/tesseract/ocr-by-upload' \
--form 'file=@"/C:/.../.../your-image.png"'
```


### Shared Folder

Send a **POST** to `http://tesseract:8080/tesseract/ocr-by-filepath` as **FORM-DATA** with **fileName** parameter as a path of image (on container).

```shell
curl --location 'http://localhost:8080/tesseract/ocr-by-filepath' \
--form 'fileName="/data/1.jpg"'
```
### URL

> [!CAUTION]
> Passing image URL to a parameter brings potential security risk, use with caution!

It is also possible to OCR images from the web using `/ocr-by-filepath` endpoint:

```shell
curl --location 'http://localhost:8080/tesseract/ocr-by-filepath' \
--form 'fileName="https://raw.githubusercontent.com/luizcarlosfaria/TesseractOCRWebApi/refs/heads/master/ocr/tests/1.jpg"'
```

To allow URLs set the corresponding environment variable to `true`:

```yaml
services:

  ocr:
    image: ghcr.io/luizcarlosfaria/tesseractocrwebapi/tesseract-ocr-aspnet-webapi:2.3.0
    ports:
    - "8080:8080"
    environment:
    - ALLOW_URLS=true
    volumes:
    - ./ocr/tests:/data
    networks:
    - ocr_net
```

## Security Considerations

For security reasons, only `/tmp/` or `/data/` directories (and children) are accepted as source image directories.

## Additional languages support

This image only supports English by default. 
To support additional languages build your own image:

1) Clone the repo:
    ```shell
    git clone https://github.com/luizcarlosfaria/TesseractOCRWebApi.git
    cd TesseractOCRWebApi
    ```

2) Specify build argument OCR_LANGS using one of the following ways:
   1) In `docker-compose.yml`:
       ```yaml
       version: '3.4'
       
       services:
         tesseractapi:
           image: ${DOCKER_REGISTRY-}tesseractapi
           build:
             context: .
             args:
               - OCR_LANGS=por+rus
             dockerfile: TesseractApi/Dockerfile
          
       ```
   2) In separate build command:
      ```shell
       docker compose build --build-arg OCR_LANGS=rus+por
      ```

3) Start:
    ```shell
    docker compose up -d
    ```
In the above examples `OCR_LANGS=por+rus` adds Russian and Portuguese languages to Tesseract along with built-in English. 
Full list of language and script codes can be found [here](https://tesseract-ocr.github.io/tessdoc/Data-Files-in-different-versions.html)

> [!NOTE]  
> To **install** special scripts or non-standard languages use `kebab-case` notation instead of `snake_case`
> 
> For example, to add Simplified Chinese language support specify `chi-sim` instead of `chi_sim`.
> 
> For scripts prepend code with `script-`: 
> to add Hangul vertical **script** (`hang_vert`) specify `script-hang-vert` 

## Usage

Add `lang` query parameter to url to indicate the languages you would like to use for OCR.
Language codes should be separated either by regular whitespace or by `+`:

```shell
curl --location 'http://localhost:8080/tesseract/ocr-by-upload?lang=eng+chi_sim' \
--form 'file=@"/C:/.../.../your-image.png"'
```
