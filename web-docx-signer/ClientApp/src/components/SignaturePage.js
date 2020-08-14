import React, { useState } from "react";
import { Form, Row, Col, Button } from "react-bootstrap";
import { saveAs } from "file-saver";

const SignaturePage = () => {
  const [isOneSignature, setIsOneSignature] = useState(true);
  const [isSigningInProcess, setIsSigningInProcess] = useState(false);

  const hadleSubmitAJAX = (e) => {
    e.preventDefault();
    const formData = new FormData(e.target);

    let request = new XMLHttpRequest();
    request.open("POST", "DocumentSignature", true);
    request.setRequestHeader("Content-Typexxxx", "multipart/form-data");
    request.onload = async () => {
      let response = await fetch(
        "DocumentSignature?fileGuid=" + request.response
      );
      let blob = await response.blob();
      saveAs(blob, "signed-docs.zip");
      setIsSigningInProcess(false);
    };
    request.send(formData);
    setIsSigningInProcess(true);

    e.target.reset();
  };

  return (
    <Form
      onSubmit={async (e) => {
        await hadleSubmitAJAX(e);
      }}
    >
      <Form.Group as={Row}>
        <Form.Label as="legend" column sm={2}>
          {isSigningInProcess ? "Ожидание" : "Выберите файлы"}
        </Form.Label>
        <Form.File name="uploads" multiple disabled={isSigningInProcess} />
      </Form.Group>
      <Form.Group as={Row}>
        <Form.Label as="legend" column sm={2}>
          Количество подписей
        </Form.Label>
        <Col sm={10}>
          <Form.Check
            type="radio"
            label="Одна печать"
            checked={isOneSignature}
            onChange={() => {
              setIsOneSignature(true);
              console.log(
                "TCL: SignaturePage -> isOneSignature",
                isOneSignature
              );
            }}
          />
          <Form.Check
            type="radio"
            label="Две печати"
            checked={!isOneSignature}
            onChange={() => {setIsOneSignature(false);
              console.log(
                "TCL: SignaturePage -> isOneSignature",
                isOneSignature
              );}}
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
      <Button type="submit" disabled={isSigningInProcess}>
        {isSigningInProcess ? "Ожидание" : "Подписать"}
      </Button>
    </Form>
  );
};

export { SignaturePage };
