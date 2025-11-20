using UnityEngine;
using DG.Tweening;
namespace DafultScript
{
    public class RocketSpawner : MonoBehaviour
    {

        [SerializeField] private Transform spawnPoint;  // 火箭生成点
        [SerializeField] private GameObject rocketPrefab;  // 火箭预制体
        [SerializeField] private float duration = 4f;  // 火箭移动时长
        [SerializeField] private float controlPointOffsetX = -5f;  // 控制点向左偏移
        [SerializeField] private float controlPointOffsetY = 5f;  // 控制点向上偏移
        [SerializeField] private int pathSteps = 50;  // 贝塞尔曲线点的分辨率

        [Space]
        [SerializeField] private GameObject explosionPrefab;

        private float timer;
        /// <summary>
        /// 生成火箭并沿着贝塞尔曲线移动
        /// </summary>
        /// <param name="targetPoint">目标点</param>
        public void SpawnRocket(Vector3 targetPoint, Glass glass)
        {
            // 确保 Z 坐标固定为 -20
            Vector3 initPos = new Vector3(spawnPoint.position.x, spawnPoint.position.y, -20);
            targetPoint.z = -20;

            // 控制点在起点和终点之间，向左偏移并抬高
            Vector3 controlPoint = new Vector3(
                (initPos.x + targetPoint.x) / 2 + controlPointOffsetX,  // 向左偏移
                Mathf.Max(initPos.y, targetPoint.y) + controlPointOffsetY,  // 向上抬高
                -(initPos.z + targetPoint.z) / 2);

            // 生成火箭
            GameObject rocket = Instantiate(rocketPrefab/*, initPos*//*, Quaternion.Euler(0, 0, 0)*/);
            rocket.transform.position = initPos;
            // 生成贝塞尔曲线路径
            Vector3[] path = GenerateBezierPath(initPos, controlPoint, targetPoint);

            // DOTween路径动画
            rocket.transform.DOPath(path, duration, PathType.Linear)
                .SetEase(Ease.InOutSine)
                .OnComplete(() =>
                {
                    GameObject explosionObj = Instantiate(explosionPrefab, glass.transform.position, Quaternion.identity);
                    Destroy(explosionObj, 1f);
                    glass.SetFalse();
                    Destroy(rocket.gameObject);
                });  // 动画结束后销毁火箭
        }

        /// <summary>
        /// 生成贝塞尔曲线路径
        /// </summary>
        /// <param name="startPoint">起点</param>
        /// <param name="controlPoint">控制点</param>
        /// <param name="endPoint">终点</param>
        /// <returns>路径点数组</returns>
        private Vector3[] GenerateBezierPath(Vector3 startPoint, Vector3 controlPoint, Vector3 endPoint)
        {
            Vector3[] path = new Vector3[pathSteps];
            for (int i = 0; i < pathSteps; i++)
            {
                float t = i / (float)(pathSteps - 1);
                path[i] = GetBezierPoint(startPoint, controlPoint, endPoint, t);
            }
            return path;
        }

        /// <summary>
        /// 计算贝塞尔曲线的某一点
        /// </summary>
        private Vector3 GetBezierPoint(Vector3 startPoint, Vector3 controlPoint, Vector3 endPoint, float t)
        {
            float u = 1 - t;
            return u * u * startPoint + 2 * u * t * controlPoint + t * t * endPoint;
        }
        /// <summary>
        /// 更新火箭的朝向，使其头朝路径的切线方向
        /// </summary>
        /// <param name="rocket">火箭物体</param>
        /// <param name="path">路径点数组</param>
    }
}