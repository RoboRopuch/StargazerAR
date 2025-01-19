# Stargazer App

![Unity](https://img.shields.io/badge/Engine-Unity-black?logo=unity)
![Platform](https://img.shields.io/badge/Platform-Android-green?logo=android)


## Overview

This project is a mobile application designed in Unity. It uses ArFoundation package ot overlay 3D models of stars and planets onto the real-world image captured by the smartphone’s camera. 

## Features

- Real-time overlay of celestial models.
- True north readings stabiliztaion.
- Data acccess in online and offline mode, facilitated by Firbease platform. 
- Body stabilized eleemnts position interpolation.
  
##  Project Structure

| **Directory**          | **Description**                                                                 |
|-----------------------|---------------------------------------------------------------------------------|
|  **Assets/**         | Core Unity assets (scripts, scenes, prefabs, materials)                         |
| ┣  **_Scripts/**      | All C# scripts controlling game logic                                           |
| ┃ ┣  **[Managers/](https://github.com/RoboRopuch/StargazerAR/tree/main/Assets/_Scripts/Managers)**   | Game managers (`GameManager`, `UnitManager`, `FirebaseManager`) |
| ┃ ┣  **[Models/](https://github.com/RoboRopuch/StargazerAR/tree/main/Assets/_Scripts/Models)**     | Classes defining celestial bodies in app  |
| ┃ ┣  **[UI/](https://github.com/RoboRopuch/StargazerAR/tree/main/Assets/_Scripts/UI)**    | All UI elements components, screen-stabilized and body-stabilized |
| ┃ ┗  **[Utils/](https://github.com/RoboRopuch/StargazerAR/tree/main/Assets/_Scripts/Utils)**  | Helper scripts and utilities                                                    |
| ┣  **Resources/**      | Graphic files used in UI and in materials for game objects                            |
| ┃ ┣  **[Prefabs/](https://github.com/RoboRopuch/StargazerAR/tree/main/Assets/Resources/Prefabs)**   | Reusable game objects, including models for stars and planets |
| ┃ ┗  **[Materials/](https://github.com/RoboRopuch/StargazerAR/tree/main/Assets/Resources/Materials)**  | Materials used by game objects|    
|  **ProjectSettings/** | Unity project settings                                                          |
|  **Packages/**       | Unity package dependencies                                                      |

## Demo
https://github.com/user-attachments/assets/76c67732-08d4-4ad7-ae43-7432ad0e2974


---
