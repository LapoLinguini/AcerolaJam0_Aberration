using System.Collections.Generic;
using UnityEngine;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

namespace DunesGameplay
{
    [RequireComponent(typeof(LineRenderer))]
    [RequireComponent(typeof(EdgeCollider2D))]
    public class LineWaves : MonoBehaviour
    {
        [SerializeField] LineRenderer lineRenderer;
        [SerializeField] EdgeCollider2D edgeCollider;

        
        [Header("Line Settings")]
        [SerializeField][Min(5)] int _resolution;
        [SerializeField] float _amplitude = 1;
        [SerializeField] float _frequency = 1;
        [SerializeField] Vector2 _xLimits = new Vector2(0, 1);
        [Range(0, 2)]
        [SerializeField] float _startPI = 0;
        [SerializeField][Min(0)] float _cameraBoundsThreshold = 0;
        [SerializeField] float _cameraOrthoSizeThreshold = 0;

        private void OnValidate()
        {
            DrawSineWave(false);
        }
        private void Update()
        {
            DrawSineWave(true);
            ResizeCameraOrthographicSize(Camera.main, transform.position, _cameraOrthoSizeThreshold);
        }
        void DrawSineWave(bool useCameraBounds)
        {
            transform.position = Vector3.zero;

            lineRenderer.positionCount = 0;

            //sets camera bounds for optimization
            float xStart = useCameraBounds ? GetCameraWidthBounds(Camera.main).x - _cameraBoundsThreshold : _xLimits.x;
            float xFinish = useCameraBounds ? GetCameraWidthBounds(Camera.main).y + _cameraBoundsThreshold : _xLimits.y;

            //drawing :3
            lineRenderer.positionCount = (int)(_resolution * (Mathf.Abs(-xStart + xFinish) / 10f));

            List<Vector2> edges = new List<Vector2>();

            for (int currentPoint = 0; currentPoint < lineRenderer.positionCount; currentPoint++)
            {
                float progress = (float)currentPoint / (lineRenderer.positionCount - 1);
                float x = Mathf.Lerp(xStart, xFinish, progress);
                float y = _amplitude * Mathf.Sin(Mathf.PI * _startPI + _frequency/10 * x);

                lineRenderer.SetPosition(currentPoint, new Vector3(x, y, 0));

                Vector3 lineRendererPoint = lineRenderer.GetPosition(currentPoint);
                edges.Add(new Vector2(lineRendererPoint.x, lineRendererPoint.y));
            }

            //colliders
            edgeCollider.SetPoints(edges);
            edgeCollider.edgeRadius = lineRenderer.startWidth / 2;
        }

        Vector2 GetCameraWidthBounds(Camera camera)
        {
            // width : height = ScreenResolution.x : ScreenResolution.y
            float halfWidth = camera.orthographicSize * Screen.currentResolution.width / Screen.currentResolution.height;

            return new Vector2(-halfWidth + camera.transform.position.x, halfWidth + camera.transform.position.x);
        }
        void ResizeCameraOrthographicSize(Camera camera, Vector3 objectWorldHeight, float cameraOrthoSizeThreshold)
        {
            //resizes the camera orthographic size to make a desired object always fit the camera view (relative to Y axis only)
            float cameraOrthoSize = Mathf.Abs(camera.transform.position.y - (objectWorldHeight.y + cameraOrthoSizeThreshold));
            cameraOrthoSize = Mathf.Clamp(cameraOrthoSize, 15f, Mathf.Infinity);
            camera.orthographicSize = cameraOrthoSize;
        }
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(new Vector2(GetCameraWidthBounds(Camera.main).x, 0), 1);
            Gizmos.DrawSphere(new Vector2(GetCameraWidthBounds(Camera.main).y, 0), 1);

            Gizmos.color = Color.green;
            Gizmos.DrawSphere(new Vector2(GetCameraWidthBounds(Camera.main).x - _cameraBoundsThreshold, 0), .9f);
            Gizmos.DrawSphere(new Vector2(GetCameraWidthBounds(Camera.main).y + _cameraBoundsThreshold, 0), .9f);

            Gizmos.color = Color.magenta;
            Gizmos.DrawSphere(new Vector3(Camera.main.transform.position.x, transform.position.y + _cameraOrthoSizeThreshold, 0) , 1f);
        }
    }
}
