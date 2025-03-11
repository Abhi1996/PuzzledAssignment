using System.Linq;
using UnityEngine;

public class DragAndDrop : MonoBehaviour
{
    private bool _isDragging = false;
    private Vector3 _offset;
    [SerializeField] private Vector3 lockedPosition;
    [SerializeField] private bool isLocked = false;
    private readonly float _distanceFromLockedState = 7f;
    private Camera _camera;
    private Vector3 _touchPos;

    // Start is called before the first frame update
    void Start()
    {
        _camera = Camera.main;
        float dist = Vector3.Distance(transform.localPosition, lockedPosition);
        if (dist < _distanceFromLockedState)
            SetLocked();
    }

    // Update is called once per frame
    void Update()
    {
        if (_isDragging && Input.GetMouseButtonUp(0) ||
            (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Ended))
        {
            Drop();
            return;
        }
        
        if (Input.GetMouseButton(0))
        {
            _touchPos = Input.mousePosition;
            if (_isDragging && (_camera != null)) 
                transform.position = _camera.ScreenToWorldPoint(_touchPos) + _offset;
        }
        else if (Input.touchCount > 0)
        {
            _touchPos = Input.GetTouch(0).position;
            if (_isDragging)
            {
                Drag(_touchPos);
            }
            else
            {
                RaycastHit2D hit = Physics2D.Raycast(_touchPos, Vector2.zero);
                if (hit.collider != null)
                {
                    Drag(_touchPos);
                }
            }
        }
            
        else
            return;
    }
    
    private void OnMouseDown()
    {
        Drag(Input.mousePosition);
    }
    
    private void OnMouseUp()
    {
        Drop();
    }
    
    private void Drag(Vector3 touchPos)
    {
        if (_camera != null && !isLocked)
        {
            _offset = transform.position - _camera.ScreenToWorldPoint(touchPos);
            _isDragging = true;
        }
    }
    
    private void Drop()
    {
        _isDragging = false;
        if (isLocked)
            return;
        
        float dist = Vector3.Distance(transform.localPosition, lockedPosition);
        if (dist < _distanceFromLockedState)
            SetLocked();
        else
        {
            Handheld.Vibrate();
            PuzzleManager.SavePuzzlePiecesInPlayerPref(this.name, transform.localPosition);
        }
    }

    
    private void SetLocked()
    {
        transform.localPosition = lockedPosition;
        isLocked = true;
        PuzzleManager.SavePuzzlePiecesInPlayerPref(this.name, transform.localPosition);
        PuzzleManager.TotalLockedPuzzlePieces++;
        PlayerPrefs.SetInt("TotalLockedPuzzlePieces", PuzzleManager.TotalLockedPuzzlePieces);
        
        if(!PuzzleManager.IsPuzzleComplete) 
            ControlLockedPieceVFX.Instance.TriggerLockedPieceVFX(this.gameObject);
    }
}
