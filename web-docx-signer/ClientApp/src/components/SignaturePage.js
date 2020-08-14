import React, { useState } from "react";
import { Form, Row, Col, Button } from "react-bootstrap";
import { saveAs } from "file-saver";

const SignaturePage = () => {
  const [isOneSignature, setIsOneSignature] = useState(true);

  const getZip = (path) =>{
    var request = new XMLHttpRequest();
      request.open("GET", "DocumentSignature"+"?path=" + path, true);
      request.onload = function() {
      // request.response.blob().then(function (myBlob) {
      //   console.log("TCL: hndleSubmitAJAX -> myBlob", myBlob)
      //   saveAs(myBlob, "download.zip");
      // });
      saveAsFunc(request.response);
      console.log("TCL: request.onload -> request.response", request.response)
      };    
      request.send();
  }

  const hadleSubmitAJAX = (e) => {
    e.preventDefault();
    const formData = new FormData(e.target);

    var request = new XMLHttpRequest();
    request.open("POST", "DocumentSignature", true);
    request.setRequestHeader('Content-Typexxxx', 'multipart/form-data');
    request.send(formData);

    request.onload = function() {
      // request.response.blob().then(function(myBlob) {
      //   saveAsFunc(myBlob,'download.zip');
      // });
      console.log("TCL: request.onload -> request.response1", request.response);
      getZip(request.response);

    };
    e.target.reset();
  };

  const saveAsFunc = (responseBody) => {
    setTimeout(() => {
      var blob = new Blob([responseBody], { type: "application/zip" });
      saveAs(blob);
    }, 2000);
  };

  return (
    <Form
      onSubmit={async (e) => {        
        await hadleSubmitAJAX(e);
      }}
    >
      <Form.Group as={Row}>
        <Form.Label as="legend" column sm={2}>
          Выберите файлы
        </Form.Label>
        <Form.File name="uploads" multiple />
      </Form.Group>
      <Form.Group as={Row}>
        <Form.Label as="legend" column sm={2}>
          Количество подписей
        </Form.Label>
        <Col sm={10}>
          <Form.Check
            type="radio"
            label="Одна печать"
            onChange={() => setIsOneSignature(true)}
          />
          <Form.Check
            type="radio"
            label="Две печати"
            onChange={() => setIsOneSignature(false)}
          />
        </Col>
      </Form.Group>
      <Form.Group as={Row}>
        <Form.Label as="legend" column sm={2}>
          Подпись 1
        </Form.Label>
        <Col sm={10}>
          <Form.Control
            type="text"
            name="firstName"
            placeholder="Введите текст подписи"
          />
        </Col>
      </Form.Group>
      {isOneSignature ? null : (
        <Form.Group as={Row}>
          <Form.Label as="legend" column sm={2}>
            Подпись 2
          </Form.Label>
          <Col sm={10}>
            <Form.Control
              type="text"
              name="secondName"
              placeholder="Введите текст подписи"
            />
          </Col>
        </Form.Group>
      )}
      <Button type="submit">Подписать</Button>
    </Form>
  );
};

export { SignaturePage };
