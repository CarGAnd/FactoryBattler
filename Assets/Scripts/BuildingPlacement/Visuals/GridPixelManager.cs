using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;

public class GridPixelManager : MonoBehaviour {

    public ComputeShader gridComputeShader;
    private ComputeBuffer positionBuffer;

    private int bufferSize = 64;
    RenderTexture renderTex;

    private void SetupRenderTex(Vector2Int texSize) {
        renderTex = new RenderTexture(texSize.x, texSize.y, 0);
        renderTex.dimension = TextureDimension.Tex2D;
        renderTex.graphicsFormat = GraphicsFormat.R8G8B8A8_UNorm;
        renderTex.wrapMode = TextureWrapMode.Repeat;
        renderTex.filterMode = FilterMode.Point;
        renderTex.useMipMap = false;
        renderTex.enableRandomWrite = true;
        renderTex.antiAliasing = 1;
        renderTex.isPowerOfTwo = false;
        renderTex.Create();

        RenderTexture.active = renderTex;
        GL.Clear(true, true, new Color(1, 1, 1, 1));
        RenderTexture.active = null;
    }

    public void Initialize(Vector2Int gridSize) {
        //Non power of two textures can have some weird sampling blurring even when using point sampling and no anti-aliasing
        //To prevent this we use the first power of two value that is larger than the grid size
        //Another way to get around this might be to use a buffer instead of a texture for this lookup
        //This should be possible in shadergraph by using the custom function node to specify the buffer, kind of like in this video
        //https://www.youtube.com/watch?v=o4QerN9wDWE
        Vector2Int powerOfTwoGridSize = FindSmallestPowerOfTwo(gridSize);
        SetupRenderTex(powerOfTwoGridSize);
        SetupCompute();

        Renderer rend = GetComponent<Renderer>();
        rend.material.SetTexture("_MaskTex", renderTex);
        rend.material.SetVector("_GridSize", new Vector4(powerOfTwoGridSize.x, powerOfTwoGridSize.y, 0, 0));
    }

    private Vector2Int FindSmallestPowerOfTwo(Vector2Int input) {
        int powOfTwo = 2;
        while(powOfTwo < input.x || powOfTwo < input.y) {
            powOfTwo *= 2;
        }
        return new Vector2Int(powOfTwo, powOfTwo);
    }

    public void TurnOffCells(List<Vector2Int> cellPositions) {
        int numIterations = Mathf.CeilToInt(cellPositions.Count / (float)bufferSize);
        for(int i = 0; i < numIterations; i++) {
            Vector2Int[] positions = new Vector2Int[bufferSize];
            int numPositionsInBatch = 0;
            for (int j = 0; j < bufferSize && j + i * bufferSize < cellPositions.Count; j++) {
                positions[j] = cellPositions[j + i * bufferSize];
                numPositionsInBatch += 1;
            }
            gridComputeShader.SetInt("numPositions", numPositionsInBatch);
            positionBuffer.SetData(positions);
            DispatchCompute(numPositionsInBatch);
        }
    }

    private void SetupCompute() {
        positionBuffer = new ComputeBuffer(bufferSize, sizeof(int) * 2, ComputeBufferType.Structured);
        gridComputeShader.SetBuffer(0, "positions", positionBuffer);
        gridComputeShader.SetTexture(0, "Result", renderTex);
    }

    private void OnDestroy() {
        positionBuffer.Dispose();
        //renderTex.Release();
    }

    private void DispatchCompute(int numPositions) {
        Vector3Int threadGroupSizes = GetThreadGroupSizes(gridComputeShader, 0);
        int numGroupsX = Mathf.CeilToInt(numPositions / (float)threadGroupSizes.x);
        int numGroupsY = Mathf.CeilToInt(1 / (float)threadGroupSizes.y);
        int numGroupsZ = Mathf.CeilToInt(1 / (float)threadGroupSizes.z);

        gridComputeShader.Dispatch(0, numGroupsX, numGroupsY, numGroupsZ);
    }

    public Vector3Int GetThreadGroupSizes(ComputeShader compute, int kernelIndex = 0) {
        uint x, y, z;
        compute.GetKernelThreadGroupSizes(kernelIndex, out x, out y, out z);
        return new Vector3Int((int)x, (int)y, (int)z);
    }
}
