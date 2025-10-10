<!-- TABLE OF CONTENTS -->
<details>
  <summary>Table of Contents</summary>
  <ol>
    <li><a href="#project-statement">Project Statement</a></li>
    <li>
      <a href="#project-scope">Getting Started</a>
      <ul>
        <li><a href="#scope">Scope</a></li>
        <li><a href="#stretch-goals">Stretch Goals</a></li>
      </ul>
    </li>
    <li><a href="#success-criteria">Success Criteria</a></li>
    <li><a href="#learning-goals">Learning Goals</a></li>
  </ol>
</details>


<!-- PROJECT STATEMENT -->
# Project Statement

This would be a small game with a minimalist style like the old school 2D shooter asteroids.

<img width="640" height="480" alt="image" src="https://github.com/user-attachments/assets/b8e66fdd-9e90-4394-b316-a7de237edd3b" />

The 3D shooter would need to allow you to be either in the cockpit or as a 3D person camera object to show off the aesthetics of the game. 

There should be an edge to the level that loops you when you hit it, but you can see the asteroids on the other side when you get to it. Fun reading about [3D torus](https://en.wikipedia.org/wiki/Torus_interconnect).

You can use the TrackIR to either control the camera OR the camera and the gun aiming direction.. 

The game inputs should only have

    Up Arrow - Go forward

    Left / Right Arrows - Pivot the ship left / right from the current camera view defined by TrackIR.

    Down Arrow - Go backwards

    Spacebar - Shoot.

Shooting asteroids should get you points that appear as a HUD on the screen. When you die the score should reset (or the player can have X number of lives defined by some properties). There should be a setting to ramp up the difficult more quickly so that we can control how long people play the game at demos. If you get a top 10 high score, then it should prompt you to enter a three letter name using the arrow keys. 

[Here is a video](https://www.youtube.com/watch?v=FgqimCRnzcg) of something similar to what that game should behave like. 

Here are some AI generated images showing a basic concept of what the visuals might look like, but the viewpoint should be either 3rd person or 1st person instead of top down. Bonus points if the styling mimics TrackIR's visual theme of orange and dark gray. 

<img width="512" height="512" alt="image" src="https://github.com/user-attachments/assets/97c92951-4285-4861-91ed-2dd231ef8959" />

<img width="512" height="512" alt="image" src="https://github.com/user-attachments/assets/88c4b89f-1002-48fe-aede-961626693ae2" />

<img width="512" height="512" alt="image" src="https://github.com/user-attachments/assets/67d78216-5375-403b-9576-d303bda947ad" />

<!-- PROJECT SCOPE -->
# Project Scope

## Scope and Deliverables

1. Create a game that functions like a 3D version of the retro game Asteroids. Use TrackIR to allow you to move the camera and/or shooting direction.
2. Minimalist ship that you can control using arrow keys from either third or first person. With a little trail behind the ship showing where you have moved.
3. Minimalist asteroid objects that end the game if they hit you. When you shoot them they should break up into two smaller asteroids. 
   - Small bursts of confetti or something when you hit asteroids especially the small ones.
4. Ship or asteroids live in a 3D torus where the sides swap to each other.
5. Button to shoot lasers out of your ship that disappear after a certain distance.
6. IMPORTANT: TrackIR head tracking support that controls where you are looking or where you are looking AND where you are shooting depending on a property.
7. Score at the top of the screen.
8. Settings window allowing you to configure things like number of lives, difficulty, remap controls to other keys, camera functionality, etc.

## Stretch Goals

9. Leader board, so that people can compete for the high score at trade shows.
10. Game fog for extra difficulty.
11. Fun extras like death explosion.
12. Levels with different types of asteroids. 
   - Example: Green asteroids that attract nearby asteroids and grow lager if they connect with other asteroids (you need to break them apart faster or leave them for last)
   - Example: Red asteroids that shoot back, so you need to be moving parallel to them in order to shoot them.

Out of Scope (Don’t do)
* Get stuck down a rabbit hole. There's probably a long list of other features that you could have for this game focus on the core features first.

# Success Criteria

At minimum, a game that looks and feels like the classic game asteroids, but in 3D and with TrackIR head tracking playing a big role in what makes the game play fun. 

This will be used for internal demos and tradeshow, so the game be short (2-5 minutes) sort of like playing old arcade games where they are trying to get you to spend as many quarters as possible by ramping up the difficulty quickly. 

# Learning Goals 

Since this is aimed at being a student project it’s important to have some learning goals associated with the project. Those are roughly summarized below. 
* Learn how to interface with camera hardware through an SDK.
* Learn how to establish requirements and stakeholders for a project to validate progress.
* Camera control mapping and game play interactions.
* Learn basic 3D model creation and/or manipulation.
* Make a game utilizing hit boxes, scoring, and user interactions. 

