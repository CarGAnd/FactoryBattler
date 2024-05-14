using System;
using System.Collections.Generic;
using UnityEngine;

public class CellMarker
{
    private Func<GameObject> CreateMarker;
    private List<GameObject> markers;

    public CellMarker(Func<GameObject> createMarkerFunction) {
        this.CreateMarker = createMarkerFunction;
        markers = new List<GameObject>();
    }

    public void MarkPositions(List<Vector2Int> positions, FactoryGrid grid) {
        SetNumActiveMarkers(positions.Count);
        for(int i = 0; i < positions.Count; i++) {
            markers[i].transform.position = grid.GetCellCenter(positions[i]);
        }
    }

    public void RemoveAllMarkers() {
        SetNumActiveMarkers(0);
    }

    private void SetNumActiveMarkers(int numActiveMarkers) {
        while(markers.Count < numActiveMarkers) {
            CreateNewMarker();
        }

        for(int i = 0; i < numActiveMarkers; i++) {
            markers[i].SetActive(true);
        }

        for(int i = numActiveMarkers; i < markers.Count; i++) {
            markers[i].SetActive(false);
        }
    }

    public GameObject GetMarker(int markerIndex) {
        return markers[markerIndex];
    }

    private void CreateNewMarker() {
        GameObject marker = CreateMarker();
        markers.Add(marker);
    }
}
