import subprocess
import sys

def remove_git_submodule(submodule_path):
    try:
        subprocess.run(['git', 'submodule', 'deinit', '-f', submodule_path], check=True)
        subprocess.run(['git', 'rm', '--cached', submodule_path], check=True)
        subprocess.run(['rm', '-rf', submodule_path], check=True)
        print(f"Submodule removed successfully: {submodule_path}")
        print("Don't forget commit and push")
    except subprocess.CalledProcessError as e:
        print(f"Failed to remove submodule: {e}")

if __name__ == "__main__":
    if len(sys.argv) != 2:
        print("Usage: python remove_submodule.py <submodule_path>")
        sys.exit(1)

    submodule_path = sys.argv[1]

    remove_git_submodule(submodule_path)    