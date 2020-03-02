board = [
    [1, 6, 0, 0, 0, 0, 2, 9, 8],
    [0, 4, 8, 2, 9, 5, 0, 0, 6],
    [0, 0, 0, 0, 8, 6, 0, 5, 0],
    [8, 0, 0, 0, 0, 9, 0, 3, 0],
    [0, 9, 3, 0, 2, 0, 8, 0, 7],
    [0, 1, 6, 0, 7, 0, 9, 2, 5],
    [9, 5, 4, 0, 6, 0, 0, 0, 0],
    [0, 0, 0, 5, 4, 0, 6, 7, 9],
    [6, 8, 0, 9, 0, 2, 5, 0, 0],
]

def print_board(bo):
    for i in range(len(bo)):
        # For this loop, the range is the length of the list within board[0]
        # Since each item in the list is another list, we are finding the length of the nested list, not the board list

        # For every 3rd row, add a separation line. Since 0 divided by any number is also 0, this also draws a line above the
        # first row
        if i % 3 == 0:
            print(" - - - - - - - - - - - - - - ")
        for j in range(len(bo[i])):
            # For every 3rd number, add a separation line
            if j % 3 == 0:
                # We don't want the next value to be on a new line, and the 'end' key prevents that
                print(" | ", end = "")
            # For each number, we'll want a space between them to see the numbers easier
            # At the end, there is no need for a space, since we'll be proceeding to the next line
            if j == 8: # last index of each nested list
                print(str(bo[i][j]) + " |")
            else:
                print(str(bo[i][j]) + " ", end = "")
    # We can add a final line to close off the box
    print(" - - - - - - - - - - - - - - ")

# This function finds the empty spaces in the board, defined by '0'. When a '0' is found, the function returns the coordinate position of the '0' space
def find_empty(bo):
    for i in range(len(bo)):
        for j in range(len(bo)):
            if bo[i][j] == 0:
                return (i, j)
    # No empty spaces found
    return None

def fill_space(bo, beginStep):
    # First, we need to find our first empty space
    # 'emptySpace' is a tuple (ordered, unchangable)
    step = beginStep + 1
    emptySpace = find_empty(bo)
    if not emptySpace:
        # No more empty spaces found. Puzzle solved!
        return True
    else:
        # Makes it easier to see when assigning positions in the board
        row, col = emptySpace

    # Fill in a number in sequence, then check the between the numbers in current box, row and column to
    # make sure the number is not repeated
    for a in range(1, 10):
        if check_valid(bo, a, emptySpace):
            bo[row][col] = a
            
            # Recursive function that runs the whole fill_space function again
            # If it returns True, it means the puzzle is solved
            if fill_space(bo, step):
                return True
            
            # If the solution isn't valid, eventually it will come out of recursion
            # Since it's not valid, we'll return the value we just set back to 0 (empty) and run the loop again
            bo[row][col] = 0

    return False

def check_valid(bo, num, pos):
    # Check in row
    for a in range(len(bo[0])):
        # Check if the number being considered matches any of the numbers in the row
        # The reason why we would want to skip our current position is during backtracking
        # By then, we've already assigned a number to this position, so we'd want to skip it, otherwise
        # it would check with itself, which would obviously return a duplicate number
        if bo[pos[0]][a] == num and pos[1] != a:
            return False

    # Check in column
    for b in range(len(bo)):
        if bo[b][pos[1]] == num and pos[0] != b:
            return False

    # Check in box
    # We are imagining each 3x3 box has its own coordinates in a larger 3x3 matrix. Top left box would be at position 0,0, 
    # next box would be 0,1, and so on

    # Floor divide rounds down the result to the nearest integer, so when the positions of the numbers in box 1 gets
    # divided by 3, the results are rounded down to 0,0 (remember that position 3,3 would already be a different box,
    # since list indexes begin with '0', not '1')
    box_x = pos[0] // 3
    box_y = pos[1] // 3

    for i in range(box_x * 3, box_x * 3 + 3):
        for j in range(box_y * 3, box_y * 3 + 3):
            if bo[i][j] == num and (i,j) != pos:
                return False

    return True

def solve(bo):
    find = find_empty(bo)
    if not find:
        return True
    else:
        row, col = find

    for i in range(1,10):
        if valid(bo, i, (row, col)):
            bo[row][col] = i

            if solve(bo):
                return True

            bo[row][col] = 0

    return False


def valid(bo, num, pos):
    # Check row
    for i in range(len(bo[0])):
        if bo[pos[0]][i] == num and pos[1] != i:
            return False

    # Check column
    for i in range(len(bo)):
        if bo[i][pos[1]] == num and pos[0] != i:
            return False

    # Check box
    box_x = pos[1] // 3
    box_y = pos[0] // 3

    for i in range(box_y*3, box_y*3 + 3):
        for j in range(box_x * 3, box_x*3 + 3):
            if bo[i][j] == num and (i,j) != pos:
                return False

    return True

# Print the board
#def print_board(bo):
#    for a in range(len(bo)):
#        # print a line after every 3rd row
#        # 'a' is automatically assigned as a counter
#        if a % 3 == 0 and a != 0:
#            print("- - - - - - - - - - - - - ")

#        for b in range(len(bo[0])):
#            # prints a line after every 3rd column
#            if b % 3 == 0 and b != 0:
#                print(" | ", end="")

#            # 8 is the last index number of each row, meaning that no space is added for the last number in each row
#            if b == 8:
#                print(bo[a][b])
#            else:
#                # By default, each print command ends with a new line. Hence, printing several values in different print
#                # statements would result in each value being printed on a new line. The 'end' key overwrites this new line
#                # with a specified character (in this case, an empty character), so that the next number is printed on the
#                # same line, rather than on a new line

#                # When we reach the end of a row (in this case, it is the nested list representing a row), we do want the next
#                # number to appear in a new line, hence we do not add the 'end' key to the print statement above

#                # Since we are adding a string character (space " ") to the value, we must remember to convert the value to a
#                # string, otherwise the values cannot be properly concatenated together
#                print(str(bo[a][b]) + " ", end="")

# Find empty spaces within the board, which is represented by '0'
def find_empty(bo):
    for i in range(len(bo)):
        for j in range(len(bo[0])):
            if bo[i][j] == 0:
                return (i, j)  # row, col
    return None

print_board(board)
fill_space(board, 0)
print("------------------------------------")
print_board(board)