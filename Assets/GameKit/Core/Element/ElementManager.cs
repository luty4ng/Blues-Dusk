using System.Collections.Generic;

namespace GameKit.Element
{
    internal sealed class ElementManager : GameKitModule, IElementManager
    {
        private readonly List<IElement> m_CachedElements;

        public ElementManager()
        {
            m_CachedElements = new List<IElement>();
        }
        internal override void Update(float elapseSeconds, float realElapseSeconds)
        {

        }

        internal override void Shutdown()
        {

        }

        public void RegisterElement(IElement element)
        {
            m_CachedElements.Add(element);
        }

        public void RemoveElement(IElement element)
        {
            m_CachedElements.Remove(element);
        }

        public void HighlightAll()
        {
            for (int i = 0; i < m_CachedElements.Count; i++)
            {
                m_CachedElements[i].OnHighlightEnter();
            }
        }

        public void StopHighlightAll()
        {
            for (int i = 0; i < m_CachedElements.Count; i++)
            {
                m_CachedElements[i].OnHighlightExit();
            }
        }
    }
}