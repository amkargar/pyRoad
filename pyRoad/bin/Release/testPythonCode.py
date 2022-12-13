# import libraries
from subprocess import Popen, PIPE
import sys

# Notice: Restricted Characters are [ ] {/s/}

# declare a list to add results to it
testsResult = []

def main():
    # get values from arguments
    filePath = sys.argv[1]
    timeOut = float(sys.argv[2])
    inputs = parseInOuts(sys.argv[3]) # converted inputs list
    outputs = parseInOuts(sys.argv[4]) # converted outputs list
    
    # a loop for tesing all inputs
    for i in range(len(inputs)):
        allInputs = '\n'.join(inputs[i])
        allOutputs = list(map(str.encode, outputs[i])) # list of outputs that converted from string values to byte values

        p = Popen(["python", filePath], stdin=PIPE, stdout=PIPE)
        out = p.communicate(input=str.encode(allInputs), timeout=timeOut)[0]

        if p.returncode == 0: # if code has no error
            testsResult.append(out.split() == allOutputs)

        else: # if code has an error
            testsResult.append("RuntimeError")
    
    # print the results
    print(*testsResult)
        
def parseInOuts(inOuts): # convert inputs & outputs from string to list
    listInOuts = []
    while inOuts != "":
        listIO = []
        
        io = inOuts[1: inOuts.index("]")]
        if "{\\s\\}" in io:
            listIO = io.split("{\\s\\}")
        else:
            listIO = [io]
        listInOuts.append(listIO)

        inOuts = inOuts[inOuts.index("]") + 1:]
    
    return listInOuts

main()