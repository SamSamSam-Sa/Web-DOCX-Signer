import React, { useState, useEffect } from "react";
import { Form, Row, Col, Button } from "react-bootstrap";

const SignaturePage = () => {
  const [isOneSignature, setIsOneSignature] = useState(true);

  var fileInput = document.getElementById('formHorizontalFileSelect');
  var fileList = [];

  useEffect(() => {
    console.log("TCL: SignaturePage -> fileInput", fileInput)
    fileList = [];
    for (var i = 0; i < fileInput.files.length; i++) {
    
     fileList.push(fileInput.files[i]);
    }
  }, [fileInput.files]);

  const sendFile = (fileList) => {
    var formData = new FormData();
    var request = new XMLHttpRequest();
    fileList.forEach( (file) =>{
      formData.set('file', file);
    })

    request.open("POST", './DocumentSignature', true);
    request.send(formData);
  };

  const hndleSubmitAJAX = async (e) => {
    e.preventDefault();
    console.log("TCL: hndleSubmitAJAX -> e", e);
    await sendFile();
    // const formData = new FormData();

    // var request = new XMLHttpRequest();
    // request.open("POST", "./DocumentSignature", true);
    // request.setRequestHeader('Content-Type', 'multipart/form-data');

    // request.upload.addEventListener('progress', function(e) {
    //   var percent_complete = (e.loaded / e.total)*100;
    //   console.log(percent_complete);
    // });

    // // AJAX request finished event
    // request.addEventListener('load', function(e) {
    //   // HTTP status message
    //   console.log(request.status);

    //   // request.response will hold the response from the server
    //   console.log(request.response);
    // });

    // request.send(formData);
  };

  return (
    <Form
      action="./DocumentSignature"
      onSubmit={(e) => hndleSubmitAJAX(e)}
      encType="multipart/formdata"
    >
      <Form.Group as={Row} controlId="formHorizontalFileSelect">
        <Form.Label as="legend" column sm={2}>
          Выберите файлы
        </Form.Label>
        <Form.File id="formHorizontalFileSelect" multiple="true" feedback />
      </Form.Group>

      <Form.Group as={Row}>
        <Form.Label as="legend" column sm={2}>
          Количество подписей
        </Form.Label>
        <Col sm={10}>
          <Form.Check
            type="radio"
            label="Одна печать"
            name="formHorizontalRadios"
            id="formHorizontalRadios1"
            checked={isOneSignature}
            onClick={() => setIsOneSignature(true)}
          />
          <Form.Check
            type="radio"
            label="Две печати"
            name="formHorizontalRadios"
            id="formHorizontalRadios2"
            checked={!isOneSignature}
            onClick={() => setIsOneSignature(false)}
          />
        </Col>
      </Form.Group>

      <Form.Group as={Row} controlId="formHorizontalSignText1">
        <Form.Label as="legend" column sm={2}>
          Подпись 1
        </Form.Label>
        <Col sm={10}>
          <Form.Control type="text" placeholder="Введите текст подписи" />
        </Col>
      </Form.Group>

      {isOneSignature ? null : (
        <Form.Group as={Row} controlId="formHorizontalSignText2">
          <Form.Label as="legend" column sm={2}>
            Подпись 2
          </Form.Label>
          <Col sm={10}>
            <Form.Control type="text" placeholder="Введите текст подписи" />
          </Col>
        </Form.Group>
      )}

      <Button type="submit">Подписать</Button>
    </Form>
  );
};

export { SignaturePage };
