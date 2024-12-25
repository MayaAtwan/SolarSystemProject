# Spaceship Simulation 🚀🌌🛰
<img src="readme.png" alt="Spaceship Simulation Project Overview" width="900" height="200">

This project is a spaceship simulation developed in Godot Engine. It features navigation and interaction within a dynamic solar system with multiple planets, each exerting gravitational influences. The spaceship supports local-axis rotation, thrust-based movement, and environmental forces like gravity and drag, with real-time HUD feedback. Additionally, player can exit the spaceship to explore planetary surfaces.

---

## 🚀 Features

### Realistic Physics 🌌
- Simulates gravitational pull from multiple planets and drag effects near planetary atmospheres.

### Spaceship Rotation Mechanics 🛰️
The spaceship's rotation is handled through the `ProcessRotation` method, enabling intuitive, local-axis-based movement. This ensures the spaceship moves relative to itself, maintaining precise control in a dynamic environment.

### Gravitational Simulation 🌠
Planets exert realistic gravitational forces based on distance and mass, affecting the spaceship's trajectory.

### Player Transition 🚶‍♂️🚀
- Players start inside the spaceship.
- Pressing **Enter** allows the player to exit the spaceship, dropping onto the planetary surface for exploration.
- Player controls are activated upon exiting the spaceship, allowing full movement on the space.
---

## 🕹️ How to Use

### Spaceship Controls
| Action               | Key         |
|-----------------------|-------------|
| **Yaw Left**          | `A`         |
| **Yaw Right**         | `D`         |
| **Pitch Up**          | `Q`         |
| **Pitch Down**        | `E`         |
| **Roll Left**         | `Z`         |
| **Roll Right**        | `C`         |
| **Thrust Forward**    | `W`         |
| **Thrust Backward**   | `S`         |
| **Exit Spaceship**    | `Space`     |

### Player Controls (Outside Spaceship)
| Action               | Key         |
|-----------------------|-------------|
| **Move Forward**      | `W`         |
| **Move Backward**     | `S`         |
| **Move Left**         | `A`         |
| **Move Right**        | `D`         |
| **Jump**              | `Space`     |

### Gameplay Instructions
1. **Spaceship Navigation**:
   - Use the spaceship controls to navigate the solar system.
   - Avoid gravitational pulls and drag to maintain trajectory.
2. **Exiting the Spaceship**:
   - Press **Enter** to exit the spaceship when near a planet.
   - The player will drop onto the planet and gain full movement control.

## Physics Details

### Gravity Formula 🌌
The gravitational force exerted on the spaceship is calculated as:
$$
F_g = \frac{-GM}{r^2} \cdot \text{Direction}
$$
Where:
- \( G \) is the gravitational constant.
- \( M \) is the mass of the planet.
- \( r \) is the distance from the spaceship to the planet.
- \( \text{Direction} \) is a normalized vector pointing towards the center of the planet.

---

### Thrust Formula 🚀
The thrust applied to the spaceship is given by:
$$
F_t = \text{Thrust Direction} \cdot \text{Base Thrust}
$$
Where:
- \( \text{Thrust Direction} \) is the direction in which the spaceship moves.
- \( \text{Base Thrust} \) is the constant thrust value.

---

### Drag Formula 🌠
The drag force experienced by the spaceship near a planetary atmosphere is:
$$
F_d = -v \cdot C_d
$$
Where:
- \( v \) is the velocity of the spaceship.
- \( C_d \) is the drag coefficient, which depends on the atmospheric density.

---

## ⚙️ Installation

To set up and run the Spaceship Simulation project, follow these steps:

### Prerequisites 🛠️
Ensure you have the following installed:

- **Godot Engine 4.0 or later**
  - Download it from the [official site](https://godotengine.org/). 🎮
- **Git**
  - Clone the repository via Git by downloading it from [Git's official website](https://git-scm.com/).

### Clone the Repository 💾
> **Important**: This project uses the **C# version** of Godot. Ensure you have the Mono version of Godot installed.

---

## 📜 Acknowledgments

#### Supervisor:
- Special thanks to **Professor Roi Poranne** for guidance and suggestions on improving the rotation mechanics and gravity simulation.

#### Creator: Maya Atwan
#### Institution: University of Haifa
#### Project Type: Semester Project in the Computer Graphics Course
#### Assets Used:
- [Sketchfab Models](https://sketchfab.com/tags/godot)
- [Pixabay Background Music](https://pixabay.com/sound-effects/search/space/)
