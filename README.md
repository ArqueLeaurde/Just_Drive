# Just_Drive
  An Endless driver game made with Unity
  
  Prototype de Jeu : Endless Driver
  Ce projet est un prototype fonctionnel d'un jeu de course de type "endless runner" développé avec Unity. Le joueur contrôle une voiture et doit conduire le plus loin possible en évitant le trafic généré dynamiquement.

Fonctionnalités Principales
🚗 Contrôle du Joueur
  Saut "Cartoon" : Le joueur peut faire sauter la carrosserie de la voiture pour esquiver des obstacles bas.
  Un système de cooldown empêche l'utilisation abusive du saut.
🤖 Système de Trafic IA
  Spawning Procédural : Un AICarSpawner fait apparaître les voitures IA devant le joueur et les désactive lorsqu'elles sont trop loin derrière, optimisant les performances grâce à un système de "pool d'objets".
  Comportement de l'IA : Les voitures IA (AIHandler) roulent à des vitesses variables, détectent les obstacles devant elles et tentent de changer de voie pour les éviter.
🏆 Cycle de Jeu et Score
  Gestionnaire de Jeu Centralisé : Un GameManager gère les états du jeu (Menu Principal, En Jeu, Game Over).
  Système de Score :
  Le score est calculé en fonction de la distance parcourue.
  Un bonus est accordé pour chaque voiture IA dépassée.
  Menus et Interface :
  Un Menu Principal avec des boutons pour "Jouer" et "Quitter".
  Un Écran de Game Over qui s'affiche après un délai pour laisser place à l'animation d'explosion, avec le score final, un bouton "Recommencer" et un pour retourner au menu.
🎨 Environnement et Interface (UI)
  Cycle Jour/Nuit Dynamique : Un TimeManager contrôle un cycle jour/nuit complet avec :
  Transition douce entre plusieurs skyboxes (jour, coucher de soleil, nuit, etc.).
  Évolution de la couleur et de l'intensité de la lumière directionnelle (soleil/lune).
  Évolution de la lumière ambiante pour des ombres colorées et une ambiance immersive.
  Interface de Jeu (HUD) :
  Affichage de la vitesse en temps réel (km/h).
  Compteur de distance parcourue en mètres.
  Compteur du nombre de voitures dépassées.
  Une barre de cooldown visuelle pour le saut du joueur.
  
  Le jeu est facilement personnalisable avec d'autres sections ou voitures, pas besoin de changer le code, il suffit de rajouter ou d'enlever des modèles selon les préférences
🎮 Contrôles
  Jouable au clavier et à la manette
  
  Action	Clavier	Manette
  Accélérer/Freiner	W / S	Stick Gauche (Haut/Bas)
  Tourner	A / D	Stick Gauche (Gauche/Droite)
  Sauter	Espace	Bouton Sud (A sur Xbox, X sur PS)
  Recommencer	R	-
  
  
  
  Prototype "Just Drive" par ArqueLeaurde
