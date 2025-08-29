using UnityEngine;

public class MagicHolder : MonoBehaviour
{
    private ParticleSystem[] childParticles;
    private int currentIndex = 0;

    void Start()
    {
        // Busca todos os ParticleSystems filhos
        childParticles = GetComponentsInChildren<ParticleSystem>(true);

        // Ativa apenas o primeiro, desativa os demais
        for (int i = 0; i < childParticles.Length; i++)
        {
            childParticles[i].gameObject.SetActive(i == currentIndex);
        }
    }

    void Update()
    {
        if (childParticles == null || childParticles.Length == 0)
            return;

        // Q: retrocede, E: avan�a
        if (Input.GetKeyDown(KeyCode.Q))
        {
            SwitchParticle(-1);
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            SwitchParticle(1);
        }
    }

    void SwitchParticle(int direction)
    {
        // Desativa o atual
        childParticles[currentIndex].gameObject.SetActive(false);

        // Calcula novo �ndice (loop circular)
        currentIndex = (currentIndex + direction + childParticles.Length) % childParticles.Length;

        // Ativa o novo
        childParticles[currentIndex].gameObject.SetActive(true);
    }

    public string GetCurrentMagicName()
    {
        if (childParticles != null && childParticles.Length > 0)
            return childParticles[currentIndex].gameObject.name;
        return "";
    }
}
