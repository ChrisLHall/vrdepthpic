using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhotoParticles : MonoBehaviour
{
    public ParticleSystem particles;
    public Texture2D photo;
    public Texture2D depth;

    public float minDepth = 3f;
    public float maxDepth = 500f;
    public float fov = 80;


    // Start is called before the first frame update
    void Start()
    {
        SpawnParticles();
    }


    [ContextMenu("Spawn particles")]
    private void SpawnParticles()
    {
        // width of the fov / distance = tan fov / 2. divide by pixels / 2
        var deltaX = Mathf.Tan(Mathf.Deg2Rad * fov / 2f) * minDepth / (photo.width / 2f);

        var particleArray = new UnityEngine.ParticleSystem.Particle[photo.height * photo.width];
        for (int y = 0; y < photo.height; y++)
        {
            for (int x = 0; x < photo.width; x++)
            {
                var particle = new UnityEngine.ParticleSystem.Particle();
                //particle.startColor = photo.GetPixel(x, y);
                var photoColor = photo.GetPixel(x, y);
                var depthColor = depth.GetPixel(x, y);
                // todo remove
                //depthColor = new Color(ColorToDepth(depthColor), 0f, 0f);
                particle.startColor = photoColor;
                var pixelDepth = ColorToDepth(depthColor, minDepth, maxDepth);
                particle.position = new Vector3((-photo.width / 2f + x) * deltaX * pixelDepth, (-photo.height / 2f + y) * deltaX * pixelDepth, pixelDepth);
                particle.startSize = 1.2f * deltaX * pixelDepth;
                particle.startLifetime = 100f;
                particle.remainingLifetime = 100f;
                particleArray[y * photo.width + x] = particle;
            }
        }
        particles.SetParticles(particleArray);

        Debug.Log($"array size {particleArray.Length} part 1 start color {particleArray[0].startColor} lifetime {particleArray[0].startLifetime} pos {particleArray[0].position}");
    }

    // returns range 0-1
    public static float ColorToDepth(Color c, float close, float far)
    {
        float h, s, v = 0;
        Color.RGBToHSV(c, out h, out s, out v);
        // use the square?
        var alpha = (1f - v);
        //return Mathf.Lerp(close, far, alpha * alpha);
        var epsilon = 1f / (far / close);
        return close * (1f / (v + epsilon));
    }
}
