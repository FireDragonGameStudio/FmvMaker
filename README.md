# FmvMaker (Experimental Graph Tool Version)

**Welcome to FmvMaker v2 (Experimental).**

![FmvMaker Logo small](https://github.com/FireDragonGameStudio/FmvMaker/blob/experimental_graph_tool_impl/Assets/FmvMaker/Textures/FmvMaker-Logo-169.png)

This branch introduces a completely new, node-based workflow for creating FMV games in Unity. It replaces the old JSON/Visual Scripting system with a custom **Graph Editor** based on Unitys [**Graph Toolkit**](https://docs.unity3d.com/Packages/com.unity.graphtoolkit@0.4/manual/index.html), making it easier to visualize and build your game logic without writing code.

> **⚠️ Warning:** This branch is currently **EXPERIMENTAL**. Features may change, and bugs may occur. Please report any issues you encounter!

---

## 📖 Quick Start Manual

Follow these simple steps to create your first FMV sequence, based on the [Sneak Peek Video](https://www.youtube.com/watch?v=Br4n8lIhe0A).

### 1. Setup Your Videos
* Import your video files into Unity.
* Place them in a valid folder (e.g., `Assets/FmvMaker/Videos` or any folder you prefer).

### 2. Create a Graph
* Right-click in the Project window.
* Select **Create > FmvMaker > FmvMaker Graph**.
* Name your graph (e.g., `NewGameGraph`).
* Double-click the asset to open the **Graph Editor**.

### 3. Build Your Flow
* **Add Nodes:** Right-click or press `Spacebar` in the editor to add nodes.
* **Start Node:** Always begin with a `Start FMV Node`.
* **Play a Video:** * Add a `Video Node`.
    * Connect the `Start Node` output to the `Video Node` input.
    * **Select Video:** Click the `Video Node`. In the Graph Inspector (right side), assign your video clip to the `Video Clip` field.
* **Create Choices:**
    * Add a `Video Context Node` (this acts as a hub for choices).
    * Click "Add Block" in the node to create multiple outputs (Choice 1, Choice 2, etc.).
    * Connect each output to different `Video Nodes`.
    * **Position Buttons:** In the Inspector, adjust `Relative Screen Position` (X/Y from 0 to 1) to place the click zones on screen.

### 4. Run the Game
* Open the `FmvMakerDemo` scene (or dublicate it and build your own scene).
* Select the `FmvMaker` GameObject.
* Locate the `FmvVideo` component in the Inspector.
* Drag your **NewGameGraph** asset into the `Runtime Graph` field.
* Press **Play** in Unity!

---

<img width="2208" height="1339" alt="FmvMaker-Overview" src="https://github.com/user-attachments/assets/31784f56-7780-4c2c-ab64-9f3def649367" />

---

## 🚀 Key Features

* **Native Graph Editor:** No more JSON editing. Visualize your entire game flow in a dedicated window.
* **Integrated Inventory:** * Create items via **Create > FmvMaker Inventory Item**.
    * **Unlock Paths:** Set "Needed Item" on a video node to restrict access until the player has the specific item.
    * **Reward Players:** Set "Giving Items" on a node to add items to the inventory upon watching.
* **Portals:** Create clean loops (e.g., returning to a dialogue menu) without messy wires crossing the screen.
* **Save/Load System:** Built-in support to save inventory state and progress automatically.

---

## 🆚 Advantages Over Version 1

| Feature | Version 1 (Legacy) | **Version 2 (Graph Tool)** |
| :--- | :--- | :--- |
| **Workflow** | Manual JSON text files | **Visual Drag-and-Drop Editor** |
| **Setup Time** | High (prone to syntax errors) | **Instant (Plug & Play)** |
| **Logic** | Generic State Machine | **Dedicated FMV Logic Nodes** |
| **Conditionals** | Hard to implement | **Built-in Item Checks** |
| **Inventory** | Basic / External | **Native & Integrated into Graph** |

---

## 🤝 Support & Contribution

If you find this tool useful or want to support its development:
* **Feedback:** Open an issue on this repository.
* **Sponsor:** Support the developer on [GitHub Sponsors](https://github.com/sponsors/FireDragonGameStudio) or [Patreon](https://patreon.com/WaveLabs).
* **Community:** Join the discussion in the comments of the [Preview Video](https://www.youtube.com/watch?v=Br4n8lIhe0A).

Happy Creating!
