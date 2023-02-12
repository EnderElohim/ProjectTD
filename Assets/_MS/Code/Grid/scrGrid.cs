using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scrGrid<TGridObject>
{
    private Vector2Int gridDimension;
    private TGridObject[,] gridArray;
    private float cellSize;
    private TextMesh[,] debugTextArray;
    private Vector3 originPosition;
    private bool isDebugOn = true;

    //Heatmap
    public const int HEAT_MAP_MAX_VALUE = 100;
    public const int HEAT_MAP_MIN_VALUE = 0;
    public event EventHandler<OnGridValueChangedEventArgs> OnGridValueChanged;
    public class OnGridValueChangedEventArgs : EventArgs
    {
        public Vector2Int pos;
    }

    public scrGrid(Vector2Int _dimension, float _cellSize, Vector3 _originPos, Func<scrGrid<TGridObject>, Vector2Int, TGridObject> _createGridObject)
    {
        gridDimension = _dimension;

        gridArray = new TGridObject[gridDimension.x, gridDimension.y];
        for (int x = 0; x < gridArray.GetLength(0); x++)
        {
            for (int y = 0; y < gridArray.GetLength(1); y++)
            {
                gridArray[x, y] = _createGridObject(this, new Vector2Int(x, y));
            }
        }


        debugTextArray = new TextMesh[gridDimension.x, gridDimension.y];
        cellSize = _cellSize;
        originPosition = _originPos;


        for (int x = 0; x < gridArray.GetLength(0); x++) //Cycle through first dimension
        {
            for (int y = 0; y < gridArray.GetLength(1); y++) //Cycle through second dimension
            {
                //debugTextArray[x,y] = scrUtilities.CreateWorldText(gridArray[x, y]?.ToString(), null, GetWorldPosition(new Vector3Int(x, 0, y)) + (new Vector3(cellSize,cellSize)*0.5f), 20, Color.white, TextAnchor.MiddleCenter);
                Debug.DrawLine(GetWorldPosition(new Vector3Int(x, 0, y)), GetWorldPosition(new Vector3Int(x, 0, y + 1)), Color.cyan, 1000f);
                Debug.DrawLine(GetWorldPosition(new Vector3Int(x, 0, y)), GetWorldPosition(new Vector3Int(x + 1, 0, y)), Color.cyan, 1000f);

            }
        }
        Debug.DrawLine(GetWorldPosition(new Vector3Int(0, 0, gridDimension.y)), GetWorldPosition(new Vector3Int(gridDimension.x, 0, gridDimension.y)), Color.cyan, 1000f);
        Debug.DrawLine(GetWorldPosition(new Vector3Int(gridDimension.x, 0, 0)), GetWorldPosition(new Vector3Int(gridDimension.x, 0, gridDimension.y)), Color.cyan, 1000f);

        OnGridValueChanged += (object sender, OnGridValueChangedEventArgs eventArgs) =>
        {
            if (isDebugOn == true)
                debugTextArray[eventArgs.pos.x, eventArgs.pos.y].text = gridArray[eventArgs.pos.x, eventArgs.pos.y]?.ToString();
        };
    }

    public int GetWidth()
    {
        return gridDimension.x;
    }

    public int GetHeight()
    {
        return gridDimension.y;
    }

    public float GetCellSize()
    {
        return cellSize;
    }

    public Vector3 GetWorldPosition(Vector3Int _pos)
    {
        return (new Vector3(_pos.x, 0 ,_pos.z) * cellSize) + originPosition;
    }

    public void WorldPosToXY(Vector3 _worldPos, out Vector2Int _pos)
    {
        _pos = new Vector2Int(Mathf.FloorToInt((_worldPos - originPosition).x / cellSize),Mathf.FloorToInt((_worldPos - originPosition).z / cellSize));
    }

    public void SetGridObject(Vector2Int _pos, TGridObject _val)
    {
        if (_pos.x < 0 || _pos.y < 0 || _pos.x >= gridDimension.x || _pos.y >= gridDimension.y) return;
        gridArray[_pos.x, _pos.y] = _val;

        //heatmap
        //gridArray[_pos.x, _pos.y] = Mathf.Clamp(gridArray[_pos.x, _pos.y], HEAT_MAP_MIN_VALUE, HEAT_MAP_MAX_VALUE);

        if (OnGridValueChanged != null) OnGridValueChanged(this, new OnGridValueChangedEventArgs { pos = _pos });

        ////Debugg
        //if(isDebugOn == true)
        //    debugTextArray[_pos.x, _pos.y].text = gridArray[_pos.x, _pos.y]?.ToString();
    }

    public void TriggerGridObjectChanged(Vector2Int _pos)
    {
        Debug.Log("TriggerGridObjectChanged");
        if (OnGridValueChanged != null)
        {
            OnGridValueChanged(this, new OnGridValueChangedEventArgs { pos = _pos });
        }

        //if (isDebugOn == true)
        //    debugTextArray[_pos.x, _pos.y].text = gridArray[_pos.x, _pos.y]?.ToString();
    }

    public void SetGridObject(Vector3 _worldPos, TGridObject _val)
    {
        Vector2Int _pos;
        WorldPosToXY(_worldPos, out _pos);
        SetGridObject(_pos, _val);
    }

    public TGridObject GetGridObject(Vector2Int _pos)
    {
        if (_pos.x < 0 || _pos.y < 0 || _pos.x >= gridDimension.x || _pos.y >= gridDimension.y) return default(TGridObject);
        return gridArray[_pos.x, _pos.y];
    }

    public TGridObject GetGridObject(Vector3 _worldPos)
    {
        Vector2Int _pos;
        WorldPosToXY(_worldPos, out _pos);
        return GetGridObject(_pos);
    }
   

}
