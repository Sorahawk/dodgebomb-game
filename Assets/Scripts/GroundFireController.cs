using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GroundFireController : CommonController {
    public int owner;

    public int getOwner() {
        return owner;
    }

    public void setOwner(int playerIndex) {
        owner = playerIndex;
    }
}
