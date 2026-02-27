using UnityEngine;


// Notice the 'partial' keyword. This tells the compiler this is part of the AnneauController class.
public partial class AnneauController 
{
    /// <summary>
    /// !!!!!!!!!!!!!!!!!!!!!! Operations of these buttons !!!!!!!!!!!!!!!!!!!!
    /// !!!!!!!!!!!!!!!!!!!!!! ADD LOGIC OF BUTTONS ON THE ANNEAU HERE !!!!!!!!
    /// </summary>
    private void ExecuteIfConfirmed(Vector2 p)
    {
        var btn = FindBtn(_activeValLayer, p, "btnConfirm", "btnCancel");
        
        if (btn != null && btn.name == "btnConfirm" && _pendingOpBtn != null) 
        {
            // --------------------------------------------------
            // DELETE Object Button
            // --------------------------------------------------
            if (_pendingOpBtn.name == "btnDelete")
            {
                if (_targetDebris != null)
                {
                    SimulationManager.Instance.RemoveDebris(_targetDebris.ObjectData.Id);
                }
                else if (_targetCatcher != null)
                {
                    // Logic to delete catcher 
                    SimulationManager.Instance.DestroyCatcher();
                }
            } 
            // --------------------------------------------------
            // SEND TO HOLOLENS Button
            // --------------------------------------------------
            else if (_pendingOpBtn.name == "btnHolo") 
            {
                if (_targetDebris != null)
                {
                    // Logic to send debris
                    HololensMessage.SendDebrisMessage(MessageCommand.SPAWN, _targetDebris.ObjectData);
                }
                else if (_targetCatcher != null)
                {
                    // Send SPAWN for the first time, UPDATE for the following times
                    MessageCommand cmd = _targetCatcher.HasBeenSpawned ? MessageCommand.UPDATE : MessageCommand.SPAWN;
                    
                    HololensMessage.SendCatcherAndDebrisMessages(
                        cmd, 
                        _targetCatcher.TargetDebris,
                        _targetCatcher.ObjectData,
                        _targetCatcher.CurrentProgressSeconds);

                    _targetCatcher.HasBeenSpawned = true;
                }
            } 
            // --------------------------------------------------
            // FOCUS Button
            // --------------------------------------------------
            else if (_pendingOpBtn.name == "btnFocas") 
            { 
                if (_targetDebris != null)
                {
                    SimulationManager.Instance.SelectDebris(_targetDebris.ObjectData.Id);
                    CameraManager.Instance.FollowDebris(_targetDebris.gameObject);
                }
                else if (_targetCatcher != null)
                {
                    SimulationManager.Instance.SelectCatcher(this._targetCatcher.ObjectData);
                    CameraManager.Instance.FollowDebris(_targetCatcher.gameObject);
                }
            }
        }
        
        // Hide the menu regardless of confirmation or cancellation
        HideMenu();
    }
}