using System;
using System.Collections.Generic;
using UnityEngine;

public class StarPuzzleBoard : MonoBehaviour
{
    private List<StarLink> _greenLinks;
    private List<StarLink> _redLinks;
    private List<StarLink> _blueLinks;
    private List<StarLink> _brownLinks;
    private List<StarLink> _purpleLinks;

    private void Start()
    {
        // CreatePuzzle();
    }

    private void CheckWin()
    {
        if (!IsStartEndLinked(_greenLinks)) return;
        if (!IsStartEndLinked(_redLinks)) return;
        if (!IsStartEndLinked(_blueLinks)) return;
        if (!IsStartEndLinked(_brownLinks)) return;
        if (!IsStartEndLinked(_purpleLinks)) return;
        
        // Subscribe manager event 
    }

    private bool IsStartEndLinked(List<StarLink> links)
    {
        bool start = false;
        bool end = false;
        
        foreach (var link in links)
        {
            if (link.StartStar.IsStartStar() || link.EndStar.IsStartStar())
            {
                start = true;
            }
            else if (link.StartStar.IsEndStar() || link.EndStar.IsEndStar())
            {
                end = true;
            }
        }
        
        return (start && end);
    }
    
    
    
}
