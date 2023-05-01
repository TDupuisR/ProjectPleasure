using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CamSpaceMovement : MonoBehaviour
{
    [Header("Required Assets")]
    [SerializeField] Transform _subCam;
    [SerializeField] Transform _camPos;
    [SerializeField] InputActionReference
        _keyMovement,
        _keyRotation,
        _pointerPosition,
        _pointerDelta,
        _rightAction,
        _miscAction, 
        _mouseScroll;

    [Header("Sensitivity Values")]
    [SerializeField] float _keyboardSpeed = 1.0f;
    [SerializeField] float _mouseTranslateSpeed = 1.0f;
    [SerializeField] float _mouserotationSpeed = 1.0f;
    [SerializeField] float _mouseScrollSpeed = 1.0f;
    [SerializeField] float _smoothMouse = 0.12f;

    Vector3 _translate;
    Vector3 _rotation;
    Vector3 _subRotation;
    Vector3 _zoom;

    float rotationCurrent;
    float subRotationCurrent;
    float translationCurrentX;
    float translationCurrentZ;
    float zoomCurrent;


    private void FixedUpdate()
    {
        Movement();
    }

    void Movement()
    {
        //Camera Zoom//
        float zoomFactor = (50f - _camPos.localPosition.z*3f) /200f;

        float zoomInput = _mouseScroll.action.ReadValue<float>() * _mouseScrollSpeed * zoomFactor;
        _zoom = new(0f, 0f, Mathf.SmoothDamp(_zoom.z, zoomInput, ref zoomCurrent, _smoothMouse));


        //Camera Position//
        if (_miscAction.action.IsPressed())
        {
            Vector2 movementInput = _pointerDelta.action.ReadValue<Vector2>() * _mouseTranslateSpeed * zoomFactor;
            _translate = new(Mathf.SmoothDamp(_translate.x, -movementInput.x, ref translationCurrentX, _smoothMouse),
                             0f,
                             Mathf.SmoothDamp(_translate.z, -movementInput.y, ref translationCurrentZ, _smoothMouse));
        }
        else if (_keyMovement.action.IsPressed())
        {
            Vector2 movementInput = _keyMovement.action.ReadValue<Vector2>() * _keyboardSpeed * zoomFactor;
            _translate = new(movementInput.x, 0f, movementInput.y);
        }
        else { _translate = Vector3.zero; }


        //Camera Rotation//
        if (_rightAction.action.IsPressed())
        {
            Vector2 rotationInput = _pointerDelta.action.ReadValue<Vector2>() * _mouserotationSpeed;
            _rotation = new(0f, Mathf.SmoothDamp(_rotation.y, rotationInput.x, ref rotationCurrent, _smoothMouse), 0f);
            _subRotation = new(Mathf.SmoothDamp(_subRotation.x, -rotationInput.y / 2.5f, ref subRotationCurrent, _smoothMouse), 0f, 0f);
        }
        else if (this._keyRotation.action.IsPressed())
        {
            float rotationInputX = this._keyRotation.action.ReadValue<float>() * _keyboardSpeed;
            _rotation = new(0f, rotationInputX *3f, 0f);
        }
        else { _rotation = Vector3.zero; _subRotation = Vector3.zero; }


        //Camera Clamp & Movement Application//
        if (_subCam.localRotation.eulerAngles.x > 180f) _subCam.localRotation = Quaternion.Euler(10f, 0f, 0f);
        _subCam.localRotation = Quaternion.Euler(Mathf.Clamp(_subCam.localRotation.eulerAngles.x, 10f, 89f), 0f, 0f);
        _camPos.localPosition = new(0f, 0f, Mathf.Clamp(_camPos.localPosition.z, -150f, -5f));

        transform.Translate(_translate);
        transform.Rotate(_rotation);
        _subCam.Rotate(_subRotation);
        _camPos.Translate(_zoom);
    }
}
