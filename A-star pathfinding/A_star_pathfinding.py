import pygame, NodeGrid, Pathfinding, os

pygame.init()
pygame.font.init()

clear = lambda : os.system('cls')

def main ():
    window = pygame.display.set_mode((1600, 900))
    pygame.display.set_caption("A* Pathfinding")
    nodeSize = 20
    gridSizeX = int(pygame.display.get_surface().get_width() / nodeSize)
    gridSizeY = int(pygame.display.get_surface().get_height() / nodeSize)
    #gridSizeX = 100
    #gridSizeY = 100
    grid = NodeGrid.NodeGrid(pygame.display.get_surface().get_size(), gridSizeX, gridSizeY, nodeSize)
    grid.createGrid()
    startNode = None
    targetNode = None
    run = True
    drag = False
    positionSet = 0     # Like a game state; 0 = set target, 1 = set start, 2 = set obstacles, 3 = all set, ready to play
       
    #clear()
    print("Set the target position")
    while(run):
        for event in pygame.event.get():
            if event.type == pygame.QUIT:
                run = False

            if positionSet == 0:    # Nothing set yet
                if event.type == pygame.MOUSEBUTTONDOWN:
                    targetNode = grid.setPoints(pygame.mouse.get_pos(), 1)
                    if targetNode != None:
                        positionSet += 1
                        print("Set the start position")
                    else:
                        print("Target node already in place")
                        positionSet += 1
            elif positionSet == 1:  # Target has been set
                if event.type == pygame.MOUSEBUTTONDOWN:
                    startNode = grid.setPoints(pygame.mouse.get_pos(), 2)
                    if startNode != None:
                        positionSet += 1
                        print("Set obstacles. This is not required. Press 'Enter' to continue")
                    else:
                        print("Start node already in place")
                        positionSet += 1
            elif positionSet == 2:  # Start point has been set
                if event.type == pygame.MOUSEBUTTONDOWN:
                    if event.button == 1:
                        drag = True
                elif event.type == pygame.MOUSEBUTTONUP:
                    if event.button == 1:
                        drag = False
                elif event.type == pygame.MOUSEMOTION:
                    if drag:
                        grid.setPoints(pygame.mouse.get_pos(), 3)
                elif event.type == pygame.KEYDOWN:
                    if event.key == pygame.K_RETURN:
                        positionSet += 1
                        print("All set! Ready for pathfinding")
                        print("Press 'Enter' to get the best path to the target")
                        print("Press 'Space' to show the process of finding the path")
            elif positionSet == 3:  # All set, ready to begin
                pathfinding = Pathfinding.PathFinding(targetNode, startNode, grid)
                if event.type == pygame.KEYDOWN:
                    if event.key == pygame.K_RETURN:        # Find path immediately
                        pathfinding.findPath()
                    elif event.key == pygame.K_SPACE:       # Show steps while running
                        pathfinding.findPath(True, window, grid)
                    elif event.key == pygame.K_BACKSPACE:   # Step-by-step solving
                        pass
                    positionSet += 1
                    print("All done. Press 'Space' to begin again")
            elif positionSet == 4:
                if event.type == pygame.KEYDOWN:
                    if event.key == pygame.K_SPACE:
                        grid.resetGrid()
                        targetNode = None
                        startNode = None
                        positionSet = 0
                        clear()
                        print("Set the target position")

        grid.drawGrid(window)
        pygame.display.update()

main()
pygame.quit()
